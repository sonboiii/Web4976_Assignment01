using System.Data;
using MEMS.Data;
using MEMS.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace MEMS.Api.Controllers;

[ApiController, Authorize(Roles = Roles.Super), Route("api/{slug}")]
public class TransfersController(ElectionDb db, ElectionDirectory directory) : ControllerBase
{
    [HttpPost("positions/{id:int}/transfer")]
    public async Task<IActionResult> Position(string slug, int id, TransferForm form)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var source = directory.Positions(directory.Tenant(User, slug)).Where(p => p.Id == id);
        if (!await source.AnyAsync()) return NotFound();
        if (!await db.Municipalities.AnyAsync(m => m.Id == form.MunicipalityId)) return BadRequest("Choose an existing municipality.");
        if (await db.Candidates.AnyAsync(c => c.PositionId == id)) return Conflict("Reassign the candidates before moving their position.");
        try
        {
            await source.ExecuteUpdateAsync(update => update.SetProperty(p => p.MunicipalityId, form.MunicipalityId));
        }
        catch (SqlException e) when (e.Number is 2601 or 2627)
        { return Conflict("That position already exists in the destination municipality."); }
        await transaction.CommitAsync();
        return NoContent();
    }

    [HttpPost("candidates/{id:int}/transfer")]
    public async Task<IActionResult> Candidate(string slug, int id, TransferForm form)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var candidate = await directory.Candidates(directory.Tenant(User, slug)).FirstOrDefaultAsync(c => c.Id == id);
        if (candidate == null) return NotFound();
        if (!await db.Positions.AnyAsync(p => p.MunicipalityId == form.MunicipalityId && p.Id == form.PositionId))
            return BadRequest("Choose a position in the destination municipality.");
        candidate.MunicipalityId = form.MunicipalityId;
        candidate.PositionId = form.PositionId!.Value;
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return NoContent();
    }
}
