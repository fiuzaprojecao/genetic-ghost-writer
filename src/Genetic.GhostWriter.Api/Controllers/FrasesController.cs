using Genetic.GhostWriter.Api.Data;
using Genetic.GhostWriter.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Genetic.GhostWriter.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FrasesController : ControllerBase
{
    private readonly ApplicationContext _context;

    public FrasesController(ApplicationContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Frase>>> GetFrases()
    {
        return await _context.Frases.ToListAsync();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Frase>> GetFrase(Guid id)
    {
        var frase = await _context.Frases.FindAsync(id);

        if (frase == null) return NotFound();

        return frase;
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutFrase(Guid id, Frase frase)
    {
        if (id != frase.Id) return BadRequest();

        _context.Entry(frase).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<Frase>> PostFrase(Frase frase)
    {
        _context.Frases.Add(frase);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetFrase", new { id = frase.Id }, frase);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteFrase(Guid id)
    {
        var frase = await _context.Frases.FindAsync(id);
        if (frase == null) return NotFound();

        _context.Frases.Remove(frase);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}