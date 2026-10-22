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
public class AdministratorsController(ApplicationDbContext db, UserManager<ApplicationUser> users) : AdminController
{
    public async Task<IActionResult> Index()
    {
        ViewBag.Municipalities = await db.Municipalities.ToDictionaryAsync(m => m.MunicipalityId, m => m.Name);

        return View(await users.Users.OrderBy(x => x.UserName).ToListAsync());
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await Options();

        return View(new AdminUserForm());
    }

    [HttpPost]
    public async Task<IActionResult> Create(AdminUserForm model)
    {
        if (!await db.Municipalities.AnyAsync(m => m.MunicipalityId == model.MunicipalityId))
            ModelState.AddModelError("MunicipalityId", "Choose an existing municipality.");

        if (!ModelState.IsValid)
        {
            await Options();

            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Username,
            MunicipalityId = model.MunicipalityId
        };
        var result = await users.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            result = await users.AddToRoleAsync(user, Roles.ForMunicipality(model.MunicipalityId));

            if (!result.Succeeded)
                await users.DeleteAsync(user);
        }

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
            await Options();

            return View(model);
        }

        TempData["Message"] = "Municipality administrator created.";

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await users.FindByIdAsync(id);

        if (user == null)
            return NotFound();

        if (await users.IsInRoleAsync(user, Roles.SuperAdmin))
            return Forbid();

        var result = await users.DeleteAsync(user);
        TempData["Message"] = result.Succeeded ? "Administrator removed." : "Unable to remove administrator.";

        return RedirectToAction("Index");
    }

    private async Task Options() => ViewBag.Municipalities = new SelectList(await db.Municipalities.OrderBy(m => m.Name)
        .ToListAsync(), "MunicipalityId", "Name");
}
