using MEMS.Data;
using MEMS.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MEMS.Api.Controllers;

[Route("api/{slug}/positions")]
public class PositionsController(ElectionDb db, ElectionDirectory directory) : ElectionsController(db, directory)
{
    [HttpGet]
    public async Task<IActionResult> List(string slug)
    {
        var tenant = Directory.Tenant(User, slug);
        if (!await tenant.AnyAsync()) return NotFound();
        return Ok(await Directory.Positions(tenant).AsNoTracking().OrderBy(p => p.Name).ToListAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(string slug, int id) =>
        await Directory.Positions(Directory.Tenant(User, slug)).FirstOrDefaultAsync(p => p.Id == id) is { } p ? Ok(p) : NotFound();

    [Authorize, HttpPost]
    public async Task<IActionResult> Create(string slug, PositionForm form)
    {
        var tenant = await Directory.Tenant(User, slug).SingleOrDefaultAsync();
        if (tenant == null) return NotFound();
        var position = new Position { MunicipalityId = tenant.Id };
        Db.Add(position).CurrentValues.SetValues(form);
        return await Save("That position already exists in this municipality.") ?? CreatedAtAction(nameof(Get), new { slug, id = position.Id }, position);
    }

    [Authorize, HttpPut("{id:int}")]
    public async Task<IActionResult> Edit(string slug, int id, PositionForm form)
    {
        var position = await Directory.Positions(Directory.Tenant(User, slug)).FirstOrDefaultAsync(p => p.Id == id);
        if (position == null) return NotFound();
        Db.Entry(position).CurrentValues.SetValues(form);
        return await Save("That position already exists in this municipality.") ?? Ok(position);
    }

    [Authorize, HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(string slug, int id)
    {
        var position = await Directory.Positions(Directory.Tenant(User, slug)).FirstOrDefaultAsync(p => p.Id == id);
        if (position == null) return NotFound();
        Db.Remove(position);
        return await Save("Delete or reassign the candidates for this position first.") ?? NoContent();
    }
}
