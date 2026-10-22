using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web4976_Assignment01.Services;
using Web4976_Assignment01.ViewModels;
using Microsoft.AspNetCore.Identity;
using Web4976_Assignment01.Data;
using Web4976_Assignment01.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Microsoft.Extensions.Options;

namespace Web4976_Assignment01.Areas.Admin.Controllers;

public class CandidatesController(ApplicationDbContext db, MunicipalScope scope, ImageStorage images) : AdminController
{
    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        var q = scope.Candidates(User);

        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(c => c.Name.ToLower().Contains(search.ToLower()));

        var count = await q.CountAsync();
        page = Math.Clamp(page, 1, Math.Max(1, (int)Math.Ceiling(count / 12d)));
        ViewBag.Search = search;
        ViewBag.Page = page;
        ViewBag.Pages = (int)Math.Ceiling(count / 12d);

        return View(await q.OrderBy(c => c.Name).Skip((page - 1) * 12).Take(12).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var c = await scope.Candidates(User).FirstOrDefaultAsync(c => c.CandidateId == id);

        return c == null ? NotFound() : View(c);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        var c = id == null ? new Candidate() : await scope.Candidates(User)
            .FirstOrDefaultAsync(c => c.CandidateId == id);

        if (c == null)
            return NotFound();

        await Options();

        return View(new CandidateForm
        {
            CandidateId = c.CandidateId,
            Name = c.Name,
            PositionId = c.PositionId,
            MunicipalityId = c.Position?.MunicipalityId ?? scope.MunicipalityId(User) ?? 0,
            Profile = c.Profile,
            Tagline = c.Tagline,
            Website = c.Website,
            Email = c.Email,
            SocialUrl = c.SocialUrl,
            VideoUrl = c.VideoUrl
        });
    }

    [HttpPost, RequestSizeLimit(3 * 1024 * 1024)]
    public async Task<IActionResult> Edit(CandidateForm model)
    {
        var c = model.CandidateId == 0 ? new Candidate() : await scope.Candidates(User)
            .FirstOrDefaultAsync(x => x.CandidateId == model.CandidateId);

        if (c == null)
            return NotFound();

        if (!scope.CanManage(User, model.MunicipalityId))
            return Forbid();

        if (!await db.Positions.AnyAsync(p => p.PositionId == model.PositionId && p.MunicipalityId == model.MunicipalityId))
            ModelState.AddModelError("PositionId", "Choose a position belonging to the selected municipality.");

        if (!ModelState.IsValid)
        {
            await Options();

            return View(model);
        }

        string? image;
        try
        {
            image = await images.SaveAsync(model.Image);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("Image", ex.Message);
            await Options();

            return View(model);
        }

        var old = c.PhotoFileName;
        c.Name = model.Name.Trim();
        c.PositionId = model.PositionId;
        c.Profile = model.Profile.Trim();
        c.Tagline = model.Tagline?.Trim() ?? "";
        c.Website = model.Website;
        c.Email = model.Email;
        c.SocialUrl = model.SocialUrl;
        c.VideoUrl = model.VideoUrl;

        if (image != null)
            c.PhotoFileName = image;

        if (c.CandidateId == 0)
            db.Candidates.Add(c);

        await db.SaveChangesAsync();

        if (image != null)
            images.Delete(old);

        TempData["Message"] = "Candidate saved.";

        return RedirectToAction("Index");
    }

    [HttpGet]
    public Task<IActionResult> Delete(int id) => Details(id);

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var c = await scope.Candidates(User).FirstOrDefaultAsync(x => x.CandidateId == id);

        if (c == null)
            return NotFound();

        db.Candidates.Remove(c);
        await db.SaveChangesAsync();
        images.Delete(c.PhotoFileName);
        TempData["Message"] = "Candidate deleted.";

        return RedirectToAction("Index");
    }

    private async Task Options()
    {
        ViewBag.Municipalities = new SelectList(await scope.Municipalities(User)
            .OrderBy(x => x.Name)
            .ToListAsync(), "MunicipalityId", "Name");
        ViewBag.Positions = await scope.Positions(User).Include(p => p.Municipality).OrderBy(x => x.Name).ToListAsync();
    }
}
