using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web4976_Assignment01.Models;
using Web4976_Assignment01.ViewModels;
using Microsoft.EntityFrameworkCore;
using Web4976_Assignment01.Data;

namespace Web4976_Assignment01.Controllers;

public class CandidatesController(ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> Details(int id)
    {
        var candidate = await db.Profiles.AsNoTracking().FirstOrDefaultAsync(c => c.CandidateId == id);

        if (candidate == null)
            return NotFound();

        return View(candidate);
    }

    public async Task<IActionResult> Compare()
    {
        var ids = Selection.Read(HttpContext.Session, "Compare");

        return View("Details", await db.Profiles.AsNoTracking()
            .Where(c => ids.Contains(c.CandidateId))
            .OrderBy(c => c.Position.Municipality.Name)
            .ThenBy(c => c.Position.Name)
            .ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> ToggleCompare(int id, string? returnUrl)
    {
        if (!await db.Candidates.AnyAsync(c => c.CandidateId == id))
            return NotFound();
        var ids = Selection.Read(HttpContext.Session, "Compare");

        if (ids.Contains(id))
            ids.Remove(id);
        else if (ids.Count < 3)
                ids.Add(id);
            else
            {
                TempData["Message"] = "Compare up to three candidates.";

                return LocalRedirect(Url.IsLocalUrl(returnUrl) ? returnUrl! : "/");
            }

        Selection.Write(HttpContext.Session, "Compare", ids);

        return LocalRedirect(Url.IsLocalUrl(returnUrl) ? returnUrl! : "/");
    }
}
