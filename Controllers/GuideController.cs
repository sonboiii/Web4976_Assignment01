using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web4976_Assignment01.Models;
using Web4976_Assignment01.ViewModels;
using Microsoft.EntityFrameworkCore;
using Web4976_Assignment01.Data;

namespace Web4976_Assignment01.Controllers;

public class GuideController(ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> Index(bool print = false)
    {
        var ids = Selection.Read(HttpContext.Session, "Guide");
        var candidates = await db.Profiles.AsNoTracking()
            .Where(c => ids.Contains(c.CandidateId))
            .OrderBy(c => c.Position.Municipality.Name)
            .ThenBy(c => c.Position.Name)
            .ThenBy(c => c.Name)
            .ToListAsync();
        Selection.Write(HttpContext.Session, "Guide", candidates.Select(c => c.CandidateId).ToHashSet());

        return View(new GuideViewModel(candidates, print));
    }

    [HttpPost]
    public async Task<IActionResult> Toggle(int id, string? returnUrl)
    {
        var candidate = await db.Candidates.Include(c => c.Position).FirstOrDefaultAsync(c => c.CandidateId == id);

        if (candidate == null)
            return NotFound();

        var ids = Selection.Read(HttpContext.Session, "Guide");

        if (!ids.Remove(id))
        {
            var count = await db.Candidates.CountAsync(c => ids.Contains(c.CandidateId) && c.PositionId == candidate.PositionId);

            if (count >= candidate.Position.SeatsAvailable)
            {
                TempData["Message"] = $"You can select up to {candidate.Position.SeatsAvailable} for {candidate.Position.Name}. Remove a selection first.";

                return LocalRedirect(Url.IsLocalUrl(returnUrl) ? returnUrl! : "/Guide");
            }

            ids.Add(id);
        }

        Selection.Write(HttpContext.Session, "Guide", ids);

        return LocalRedirect(Url.IsLocalUrl(returnUrl) ? returnUrl! : "/Guide");
    }

    [HttpPost]
    public IActionResult Clear()
    {
        HttpContext.Session.Remove("Guide");

        return RedirectToAction("Index");
    }
}
