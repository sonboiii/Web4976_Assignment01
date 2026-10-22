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

public class DashboardController(MunicipalScope scope) : AdminController
{
    public async Task<IActionResult> Index() => View(new DashboardViewModel(await scope.Municipalities(User)
        .CountAsync(), await scope.Positions(User)
        .CountAsync(), await scope.Candidates(User)
        .CountAsync()));
}
