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

namespace Web4976_Assignment01.Services;

public class MunicipalClaimsFactory(UserManager<ApplicationUser> users, RoleManager<IdentityRole> roles, IOptions<IdentityOptions> options) : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>(users, roles, options)
{
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        if (user.MunicipalityId is int id && await UserManager.IsInRoleAsync(user, Roles.ForMunicipality(id)))
        {
            identity.AddClaim(new Claim("MunicipalityId", id.ToString()));
            identity.AddClaim(new Claim("MunicipalityAdmin", "true"));
        }

        return identity;
    }
}
