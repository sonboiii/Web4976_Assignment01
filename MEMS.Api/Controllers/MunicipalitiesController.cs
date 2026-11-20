using MEMS.Data;
using MEMS.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MEMS.Api.Controllers;

[Route("api/municipalities")]
public class MunicipalitiesController(ElectionDb db, ElectionDirectory directory) : ElectionsController(db, directory)
{
    [HttpGet]
    public Task<List<Municipality>> List() => Directory.Municipalities(User).AsNoTracking().OrderBy(m => m.Name).ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Municipality>> Get(int id) =>
        await Directory.Municipalities(User).FirstOrDefaultAsync(m => m.Id == id) is { } m ? Ok(m) : NotFound();

    [HttpGet("/api/{slug}/municipality")]
    public async Task<ActionResult<Municipality>> BySlug(string slug) =>
        await Directory.Tenant(User, slug).FirstOrDefaultAsync() is { } m ? Ok(m) : NotFound();

    [Authorize(Roles = Roles.Super), HttpPost]
    public async Task<IActionResult> Create(MunicipalityForm form)
    {
        var m = new Municipality();
        Db.Add(m).CurrentValues.SetValues(form);
        return await Save("That name or URL is already in use.") ?? CreatedAtAction(nameof(Get), new { id = m.Id }, m);
    }

    [Authorize, HttpPut("{id:int}")]
    public async Task<IActionResult> Edit(int id, MunicipalityForm form)
    {
        var m = await Directory.Municipalities(User).FirstOrDefaultAsync(x => x.Id == id);
        if (m == null) return NotFound();
        Db.Entry(m).CurrentValues.SetValues(form);
        return await Save("That name or URL is already in use.") ?? Ok(m);
    }

    [Authorize(Roles = Roles.Super), HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var m = await Db.Municipalities.FindAsync(id);
        if (m == null) return NotFound();
        Db.Remove(m);
        return await Save("Remove the municipality’s positions and administrator assignments first.") ?? NoContent();
    }

    [Authorize, HttpGet("/api/dashboard")]
    public async Task<Totals> Dashboard()
    {
        var tenants = Directory.Municipalities(User);
        return new(await tenants.CountAsync(), await Directory.Positions(tenants).CountAsync(), await Directory.Candidates(tenants).CountAsync());
    }
}
