using Agenda2.Database.Context;
using Agenda2.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agenda2.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PizzaController : ControllerBase
{
    private readonly AppDbContext _context;

    public PizzaController(AppDbContext context) { _context = context; }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PizzaDto>>> Get()
    {
        var pizzas = await _context.Pizzas
            .Where(p => !p.IsDeleted)
            .Select(p => new PizzaDto
            {
                Id = p.Id,
                Name = p.Name,
                Ingredients = p.Ingredients,
                Price = p.Price,
                ImageUrl = p.ImageUrl
            }).ToListAsync();

        return Ok(pizzas);
    }
}