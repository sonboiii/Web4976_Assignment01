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

[Authorize(Roles = Roles.SuperAdmin)]
public class MunicipalitiesController(ApplicationDbContext db, ImageStorage images, RoleManager<IdentityRole> roles) : AdminController
{
    public async Task<IActionResult> Index() => View(await db.Municipalities.OrderBy(m => m.Name).ToListAsync());

    public async Task<IActionResult> Details(int id)
    {
        var m = await db.Municipalities.Include(x => x.Positions)
            .ThenInclude(p => p.Candidates)
            .FirstOrDefaultAsync(x => x.MunicipalityId == id);

        return m == null ? NotFound() : View(m);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return View(new MunicipalityForm());
        var m = await db.Municipalities.FindAsync(id);

        if (m == null)
            return NotFound();

        return View(new MunicipalityForm
        {
            MunicipalityId = m.MunicipalityId,
            Name = m.Name,
            Province = m.Province,
            Description = m.Description,
            ElectionDate = m.ElectionDate
        });
    }

    [HttpPost, RequestSizeLimit(3 * 1024 * 1024)]
    public async Task<IActionResult> Edit(MunicipalityForm model)
    {
        model.Name = model.Name?.Trim() ?? "";
        var m = model.MunicipalityId == 0 ? new Municipality() : await db.Municipalities.FindAsync(model.MunicipalityId);

        if (m == null)
            return NotFound();

        if (await db.Municipalities.AnyAsync(x => x.Name == model.Name && x.MunicipalityId != model.MunicipalityId))
            ModelState.AddModelError("Name", "A municipality with this name already exists.");

        if (!ModelState.IsValid)
        {
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

            return View(model);
        }

        var old = m.LogoFileName;
        m.Name = model.Name.Trim();
        m.Province = model.Province.Trim();
        m.Description = model.Description.Trim();
        m.ElectionDate = model.ElectionDate!.Value;

        if (image != null)
            m.LogoFileName = image;

        if (m.MunicipalityId == 0)
            db.Municipalities.Add(m);

        await db.SaveChangesAsync();
        var role = Roles.ForMunicipality(m.MunicipalityId);

        if (!await roles.RoleExistsAsync(role))
            await roles.CreateAsync(new IdentityRole(role));

        if (image != null)
            images.Delete(old);

        TempData["Message"] = "Municipality saved.";

        return RedirectToAction("Index");
    }

    [HttpGet]
    public Task<IActionResult> Delete(int id) => Details(id);

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var m = await db.Municipalities.FindAsync(id);

        if (m == null)
            return NotFound();

        if (await db.Positions.AnyAsync(x => x.MunicipalityId == id) || await db.Users.AnyAsync(x => x.MunicipalityId == id))
        {
            TempData["Message"] = "Remove the municipality's positions and administrator assignments before deleting it.";

            return RedirectToAction("Index");
        }

        db.Municipalities.Remove(m);
        await db.SaveChangesAsync();
        images.Delete(m.LogoFileName);
        var role = await roles.FindByNameAsync(Roles.ForMunicipality(id));

        if (role != null)
            await roles.DeleteAsync(role);

        TempData["Message"] = "Municipality deleted.";

        return RedirectToAction("Index");
    }
}
