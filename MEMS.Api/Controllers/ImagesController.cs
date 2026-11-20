using System.Net;
using System.Text;
using MEMS.Data;
using MEMS.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MEMS.Api.Controllers;

[Route("api/{slug}")]
public class ImagesController(ElectionDb db, ElectionDirectory directory) : ElectionsController(db, directory)
{
    [HttpGet("logo")]
    public async Task<IActionResult> Logo(string slug)
    {
        var tenant = Directory.Tenant(User, slug);
        var m = await tenant.SingleOrDefaultAsync();
        if (m == null) return NotFound();
        var bytes = await Db.Logos.Where(x => x.MunicipalityId == m.Id).Select(x => x.Bytes).SingleOrDefaultAsync();
        if (bytes != null) return Picture(bytes);
        var initials = string.Concat(m.Name.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2).Select(s => s[0]));
        var svg = $"""<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 80 80"><rect width="80" height="80" rx="8" fill="{WebUtility.HtmlEncode(m.Accent)}"/><text x="40" y="52" text-anchor="middle" font-family="sans-serif" font-size="34" fill="white">{WebUtility.HtmlEncode(initials)}</text></svg>""";
        return File(Encoding.UTF8.GetBytes(svg), "image/svg+xml");
    }

    [HttpGet("candidates/{id:int}/photo")]
    public async Task<IActionResult> Photo(string slug, int id)
    {
        var c = await Directory.Candidates(Directory.Tenant(User, slug)).SingleOrDefaultAsync(c => c.Id == id);
        if (c == null) return NotFound();
        var bytes = await Db.Photos.Where(x => x.CandidateId == id).Select(x => x.Bytes).SingleOrDefaultAsync();
        if (bytes != null) return Picture(bytes);
        var stream = typeof(ImagesController).Assembly.GetManifestResourceStream($"MEMS.Api.Portraits.{c.PortraitNumber ?? 0}.svg");
        return File(stream!, "image/svg+xml");
    }

    [Authorize, HttpPut("logo"), RequestSizeLimit(ElectionImage.MaxBytes)]
    public async Task<IActionResult> UploadLogo(string slug)
    {
        await using var transaction = await Db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        var tenant = await Directory.Tenant(User, slug).SingleOrDefaultAsync();
        if (tenant == null) return NotFound();
        var bytes = await ReadImage();
        if (bytes == null) return BadRequest(ElectionImage.Requirements);
        var logo = await Db.Logos.FindAsync(tenant.Id) ?? Db.Logos.Add(new() { MunicipalityId = tenant.Id }).Entity;
        logo.Bytes = bytes;
        if (await Save("The municipality changed. Reload and retry the upload.") is { } error) return error;
        await transaction.CommitAsync();
        return NoContent();
    }

    [Authorize, HttpPut("candidates/{id:int}/photo"), RequestSizeLimit(ElectionImage.MaxBytes)]
    public async Task<IActionResult> UploadPhoto(string slug, int id)
    {
        await using var transaction = await Db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        if (!await Directory.Candidates(Directory.Tenant(User, slug)).AnyAsync(c => c.Id == id)) return NotFound();
        var bytes = await ReadImage();
        if (bytes == null) return BadRequest(ElectionImage.Requirements);
        var photo = await Db.Photos.FindAsync(id) ?? Db.Photos.Add(new() { CandidateId = id }).Entity;
        photo.Bytes = bytes;
        if (await Save("The candidate changed. Reload and retry the upload.") is { } error) return error;
        await transaction.CommitAsync();
        return NoContent();
    }

    private IActionResult Picture(byte[] bytes) => File(bytes, ElectionImage.ContentType(bytes)!);

    private async Task<byte[]?> ReadImage()
    {
        using var buffer = new MemoryStream();
        await Request.Body.CopyToAsync(buffer, HttpContext.RequestAborted);
        var bytes = buffer.ToArray();
        return ElectionImage.ContentType(bytes) is { } mime && mime == Request.ContentType ? bytes : null;
    }
}
