using MEMS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace MEMS.Api.Controllers;

[ApiController]
public abstract class ElectionsController(ElectionDb db, ElectionDirectory directory) : ControllerBase
{
    protected ElectionDb Db => db;
    protected ElectionDirectory Directory => directory;

    protected async Task<ActionResult?> Save(string conflictMessage)
    {
        try { await Db.SaveChangesAsync(); return null; }
        catch (DbUpdateConcurrencyException) { return Conflict("This record changed. Reload and try again."); }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 or 547 })
        {
            return Conflict(conflictMessage);
        }
    }
}
