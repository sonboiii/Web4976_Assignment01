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

public class MunicipalScope(ApplicationDbContext db)
{
    public bool IsSuper(ClaimsPrincipal user) => user.IsInRole(Roles.SuperAdmin);

    public int? MunicipalityId(ClaimsPrincipal user) => int.TryParse(user.FindFirstValue("MunicipalityId"), out var id) ? id : null;

    public bool CanManage(ClaimsPrincipal user, int municipalityId) => IsSuper(user) || (MunicipalityId(user) == municipalityId && user.IsInRole(Roles.ForMunicipality(municipalityId)));

    public IQueryable<Municipality> Municipalities(ClaimsPrincipal u) => IsSuper(u) ? db.Municipalities : db.Municipalities.Where(x => x.MunicipalityId == MunicipalityId(u));

    public IQueryable<Position> Positions(ClaimsPrincipal u) => IsSuper(u) ? db.Positions : db.Positions.Where(x => x.MunicipalityId == MunicipalityId(u));

    public IQueryable<Candidate> Candidates(ClaimsPrincipal u) => IsSuper(u) ? db.Profiles : db.Profiles.Where(x => x.Position.MunicipalityId == MunicipalityId(u));
}
