using System.Security.Claims;
using MEMS.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MEMS.Data;

public sealed class ElectionClaimsFactory(UserManager<ElectionUser> users, RoleManager<IdentityRole> roles,
    IOptions<IdentityOptions> options) : UserClaimsPrincipalFactory<ElectionUser, IdentityRole>(users, roles, options)
{
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ElectionUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        if (user.MunicipalityId is int id)
            identity.AddClaim(new(Roles.TenantClaim, id.ToString()));
        return identity;
    }
}

public static class Accounts
{
    public static async Task SeedAsync(ElectionDb db, UserManager<ElectionUser> users, RoleManager<IdentityRole> roles,
        string? password)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        foreach (var role in new[] { Roles.Super, Roles.Municipal })
            if (!await roles.RoleExistsAsync(role))
                Check(await roles.CreateAsync(new(role)));

        if (password != null && !await db.Users.AnyAsync())
        {
            var tenants = await db.Municipalities.ToDictionaryAsync(m => m.Slug, m => m.Id);
            await Add("super", null);
            await Add("pm", tenants["pitt-meadows"]);
            foreach (var slug in new[] { "burnaby", "surrey", "richmond" })
                await Add(slug + "admin", tenants[slug]);
        }
        await transaction.CommitAsync();

        async Task Add(string name, int? tenant)
        {
            var user = new ElectionUser { UserName = name, MunicipalityId = tenant };
            Check(await users.CreateAsync(user, password!));
            Check(await users.AddToRoleAsync(user, tenant.HasValue ? Roles.Municipal : Roles.Super));
        }
    }

    private static void Check(IdentityResult result)
    {
        if (!result.Succeeded) throw new InvalidOperationException(string.Join(" ", result.Errors.Select(e => e.Description)));
    }
}
