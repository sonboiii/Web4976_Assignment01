using MEMS.Data;
using MEMS.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MEMS.Api.Controllers;

[Route("api/{slug}/candidates")]
public class CandidatesController(ElectionDb db, ElectionDirectory directory) : ElectionsController(db, directory)
{
    [HttpGet]
    public async Task<IActionResult> List(string slug, string? search, int? positionId)
    {
        var tenant = Directory.Tenant(User, slug);
        if (!await tenant.AnyAsync()) return NotFound();
        var query = Directory.Candidates(tenant).AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(c => c.Name.Contains(search.Trim()));
        if (positionId.HasValue) query = query.Where(c => c.PositionId == positionId);
        return Ok(await query.OrderBy(c => c.Name).ThenBy(c => c.Id).ToListAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(string slug, int id) =>
        await Directory.Candidates(Directory.Tenant(User, slug)).FirstOrDefaultAsync(c => c.Id == id) is { } c ? Ok(c) : NotFound();

    [Authorize, HttpPost]
    public Task<IActionResult> Create(string slug, CandidateForm form) => Write(slug, null, form);

    [Authorize, HttpPut("{id:int}")]
    public Task<IActionResult> Edit(string slug, int id, CandidateForm form) => Write(slug, id, form);

    private async Task<IActionResult> Write(string slug, int? id, CandidateForm form)
    {
        var tenant = Directory.Tenant(User, slug);
        var m = await tenant.SingleOrDefaultAsync();
        if (m == null) return NotFound();
        var c = id.HasValue ? await Directory.Candidates(tenant).FirstOrDefaultAsync(c => c.Id == id)
            : new Candidate { MunicipalityId = m.Id, DateCreated = DateTime.UtcNow };
        if (c == null) return NotFound();
        if (!await Directory.Positions(tenant).AnyAsync(p => p.Id == form.PositionId))
            return BadRequest("Choose a position in this municipality.");
        if (!id.HasValue) Db.Add(c);
        Db.Entry(c).CurrentValues.SetValues(form);
        return await Save("The selected position is no longer available.") ??
            (id.HasValue ? Ok(c) : CreatedAtAction(nameof(Get), new { slug, id = c.Id }, c));
    }

    [Authorize, HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(string slug, int id)
    {
        var c = await Directory.Candidates(Directory.Tenant(User, slug)).FirstOrDefaultAsync(c => c.Id == id);
        if (c == null) return NotFound();
        Db.Remove(c);
        return await Save("This candidate could not be removed.") ?? NoContent();
    }
}
