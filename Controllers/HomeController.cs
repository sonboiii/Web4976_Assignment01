using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web4976_Assignment01.Models;
using Web4976_Assignment01.ViewModels;
using Microsoft.EntityFrameworkCore;
using Web4976_Assignment01.Data;

namespace Web4976_Assignment01.Controllers;

public class HomeController(ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> Index(int? municipalityId, int? positionId, string? search, string sort = "name", int page = 1)
    {
        var query = db.Profiles.AsNoTracking();

        if (municipalityId.HasValue)
            query = query.Where(c => c.Position.MunicipalityId == municipalityId);

        if (positionId.HasValue)
            query = query.Where(c => c.PositionId == positionId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();
            query = query.Where(c => c.Name.ToLower().Contains(search.ToLower()));
        }

        var total = await query.CountAsync();
        page = Math.Clamp(page, 1, Math.Max(1, (int)Math.Ceiling(total / 6d)));
        query = sort == "newest" ? query.OrderByDescending(c => c.DateCreated)
            .ThenBy(c => c.CandidateId) : query.OrderBy(c => c.Name)
            .ThenBy(c => c.CandidateId);

        return View(new DirectoryViewModel
        {
            Candidates = await query.Skip((page - 1) * 6)
                .Take(6)
                .ToListAsync(),
            Municipalities = await db.Municipalities.AsNoTracking()
                .OrderBy(m => m.Name)
                .ToListAsync(),
            Positions = await db.Positions.AsNoTracking()
                .Include(p => p.Municipality)
                .OrderBy(p => p.Name)
                .ToListAsync(),
            MunicipalityId = municipalityId,
            PositionId = positionId,
            Search = search,
            Sort = sort,
            Page = page,
            Total = total,
            SelectedIds = Selection.Read(HttpContext.Session, "Guide"),
            CompareIds = Selection.Read(HttpContext.Session, "Compare")
        });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        Response.StatusCode = 500;

        return View("~/Views/Shared/Message.cshtml");
    }
}
