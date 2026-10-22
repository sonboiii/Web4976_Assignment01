using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web4976_Assignment01.Models;
using Web4976_Assignment01.ViewModels;
using Microsoft.EntityFrameworkCore;
using Web4976_Assignment01.Data;

namespace Web4976_Assignment01.Controllers;

public class MunicipalitiesController(ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> Index() => View(await db.Municipalities.AsNoTracking()
        .Include(m => m.Positions)
        .OrderBy(m => m.Name)
        .ToListAsync());
}
