using System.Security.Claims;
using MEMS.Shared;
using Microsoft.EntityFrameworkCore;

namespace MEMS.Data;

// Every lookup, including image and write lookups, starts with these scoped queries.
public sealed class ElectionDirectory(ElectionDb db)
{
    public IQueryable<Municipality> Municipalities(ClaimsPrincipal user)
    {
        if (user.Identity?.IsAuthenticated != true || user.IsInRole(Roles.Super))
            return db.Municipalities;
        int? id = user.IsInRole(Roles.Municipal) && int.TryParse(user.FindFirstValue(Roles.TenantClaim), out var tenant) ? tenant : null;
        return db.Municipalities.Where(m => m.Id == id);
    }

    public IQueryable<Municipality> Tenant(ClaimsPrincipal user, string slug)
        => Municipalities(user).Where(m => m.Slug == slug.ToLowerInvariant());

    public IQueryable<Position> Positions(IQueryable<Municipality> tenants)
        => db.Positions.Where(p => tenants.Any(m => m.Id == p.MunicipalityId));

    public IQueryable<Candidate> Candidates(IQueryable<Municipality> tenants)
        => db.Candidates.Where(c => tenants.Any(m => m.Id == c.MunicipalityId));
}
