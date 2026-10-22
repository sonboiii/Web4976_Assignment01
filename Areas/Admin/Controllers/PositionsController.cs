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

public class PositionsController(ApplicationDbContext db, MunicipalScope scope) : AdminController
{
    public async Task<IActionResult> Index() => View(await scope.Positions(User)
        .Include(p => p.Municipality)
        .Include(p => p.Candidates)
        .OrderBy(p => p.Municipality.Name)
        .ThenBy(p => p.Name)
        .ToListAsync());

    public async Task<IActionResult> Details(int id)
    {
        var p = await scope.Positions(User)
            .Include(p => p.Municipality)
            .Include(p => p.Candidates)
            .FirstOrDefaultAsync(p => p.PositionId == id);

        return p == null ? NotFound() : View(p);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        var p = id == null ? new Position
        {
            MunicipalityId = scope.MunicipalityId(User) ?? 0
        }

        : await scope.Positions(User).FirstOrDefaultAsync(p => p.PositionId == id);

        if (p == null)
            return NotFound();

        await Options();

        return View(new PositionForm
        {
            PositionId = p.PositionId,
            Name = p.Name,
            MunicipalityId = p.MunicipalityId,
            SeatsAvailable = p.SeatsAvailable
        });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(PositionForm model)
    {
        model.Name = model.Name?.Trim() ?? "";
        var p = model.PositionId == 0 ? new Position() : await scope.Positions(User)
            .FirstOrDefaultAsync(x => x.PositionId == model.PositionId);

        if (p == null)
            return NotFound();

        if (!scope.CanManage(User, model.MunicipalityId))
            return Forbid();

        if (!await db.Municipalities.AnyAsync(m => m.MunicipalityId == model.MunicipalityId))
            ModelState.AddModelError("MunicipalityId", "Choose an existing municipality.");

        if (p.PositionId != 0 && p.MunicipalityId != model.MunicipalityId && await db.Candidates.AnyAsync(c => c.PositionId == p.PositionId))
            ModelState.AddModelError("MunicipalityId", "A position with candidates cannot be moved to another municipality.");

        if (await db.Positions.AnyAsync(x => x.MunicipalityId == model.MunicipalityId && x.Name == model.Name && x.PositionId != model.PositionId))
            ModelState.AddModelError("Name", "This position already exists in the municipality.");

        if (!ModelState.IsValid)
        {
            await Options();

            return View(model);
        }

        p.Name = model.Name.Trim();
        p.MunicipalityId = model.MunicipalityId;
        p.SeatsAvailable = model.SeatsAvailable;

        if (p.PositionId == 0)
            db.Positions.Add(p);

        await db.SaveChangesAsync();
        TempData["Message"] = "Position saved.";

        return RedirectToAction("Index");
    }

    [HttpGet]
    public Task<IActionResult> Delete(int id) => Details(id);

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var p = await scope.Positions(User).FirstOrDefaultAsync(x => x.PositionId == id);

        if (p == null)
            return NotFound();

        if (await db.Candidates.AnyAsync(c => c.PositionId == id))
        {
            TempData["Message"] = "Delete or reassign this position's candidates first.";

            return RedirectToAction("Index");
        }

        db.Positions.Remove(p);
        await db.SaveChangesAsync();
        TempData["Message"] = "Position deleted.";

        return RedirectToAction("Index");
    }

    private async Task Options() => ViewBag.Municipalities = new SelectList(await scope.Municipalities(User)
        .OrderBy(x => x.Name)
        .ToListAsync(), "MunicipalityId", "Name");
}
