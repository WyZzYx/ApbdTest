using Application;
using Application.DTO;
using Microsoft.AspNetCore.Mvc;

namespace TavernSystem;

[ApiController]
[Route("api/[controller]")]
public class AdventurersController : ControllerBase
{
    private readonly IAdventurerService _svc;
    public AdventurersController(IAdventurerService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var all = await _svc.GetAllAsync();
        return Ok(all);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var adv = await _svc.GetByIdAsync(id);
        return adv is null ? NotFound() : Ok(adv);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateAdventurerRequest req)
    {
        try
        {
            var newId = await _svc.CreateAsync(req);
            return CreatedAtAction(nameof(Get), new { id = newId }, null);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("bounty"))
        {
            return StatusCode(403, ex.Message);
        }
        catch (InvalidOperationException)
        {
            return Conflict();
        }
    }
}