using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web4976_Assignment01.Models;
using Web4976_Assignment01.ViewModels;
using Microsoft.EntityFrameworkCore;
using Web4976_Assignment01.Data;

namespace Web4976_Assignment01.Controllers;

public class AccountController(SignInManager<ApplicationUser> signIn) : Controller
{
    [HttpGet]
    public IActionResult Login(string? returnUrl) => View(new LoginViewModel { ReturnUrl = returnUrl });

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);
        var result = await signIn.PasswordSignInAsync(model.Username, model.Password, false, true);

        if (result.Succeeded)
            return Url.IsLocalUrl(model.ReturnUrl) ? LocalRedirect(model.ReturnUrl!) : RedirectToAction("Index", "Dashboard", new { area = "Admin" });

        ModelState.AddModelError("", result.IsLockedOut ? "Too many attempts. Try again in five minutes." : "The username or password is incorrect.");

        return View(model);
    }

    [Authorize, HttpPost]
    public async Task<IActionResult> Logout()
    {
        await signIn.SignOutAsync();

        return RedirectToAction("Index", "Home");
    }

    public IActionResult Denied()
    {
        Response.StatusCode = 403;

        return View("~/Views/Shared/Message.cshtml");
    }
}
