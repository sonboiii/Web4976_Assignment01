using System.Security.Claims;
using MEMS.Data;
using MEMS.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MEMS.Api.Controllers;

[ApiController, Route("api/account")]
public class AccountController(ElectionDb db, UserManager<ElectionUser> users, SignInManager<ElectionUser> signIn) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(Credentials input)
    {
        var user = await users.FindByNameAsync(input.Username);
        if (user == null || !(await signIn.CheckPasswordSignInAsync(user, input.Password, lockoutOnFailure: true)).Succeeded)
            return Unauthorized();
        var principal = await signIn.CreateUserPrincipalAsync(user);
        return SignIn(principal, IdentityConstants.BearerScheme);
    }

    [Authorize, HttpGet("me")]
    public Account Me() => new(User.Identity!.Name!, User.IsInRole(Roles.Super) ? Roles.Super : Roles.Municipal,
        int.TryParse(User.FindFirstValue(Roles.TenantClaim), out var id) ? id : null);

    [Authorize(Roles = Roles.Super), HttpGet("/api/administrators")]
    public Task<List<Administrator>> List() => users.Users.OrderBy(u => u.UserName)
        .Select(u => new Administrator(u.Id, u.UserName!, u.MunicipalityId)).ToListAsync();

    [Authorize(Roles = Roles.Super), HttpPost("/api/administrators")]
    public async Task<IActionResult> Create(AdministratorForm form)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        if (!await db.Municipalities.AnyAsync(m => m.Id == form.MunicipalityId))
            return BadRequest("Choose an existing municipality.");
        var user = new ElectionUser { UserName = form.Username, MunicipalityId = form.MunicipalityId };
        var result = await users.CreateAsync(user, form.Password);
        if (result.Succeeded) result = await users.AddToRoleAsync(user, Roles.Municipal);
        if (!result.Succeeded) return BadRequest(string.Join(" ", result.Errors.Select(e => e.Description)));
        await transaction.CommitAsync();
        return StatusCode(201, new Administrator(user.Id, user.UserName!, user.MunicipalityId));
    }

    [Authorize(Roles = Roles.Super), HttpDelete("/api/administrators/{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await users.FindByIdAsync(id);
        if (user == null) return NotFound();
        if (await users.IsInRoleAsync(user, Roles.Super)) return Forbid();
        return (await users.DeleteAsync(user)).Succeeded ? NoContent() : Conflict("Reload the administrator list and try again.");
    }
}
