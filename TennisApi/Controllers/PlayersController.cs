using Microsoft.AspNetCore.Mvc;
using TennisApi.Models;
using TennisApi.Services;

namespace TennisApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PlayersController : ControllerBase
{
    private readonly IPlayerService _service;

    public PlayersController(IPlayerService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetPlayers()
    {
        var players = await _service.GetAllAsync();
        return Ok(players);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPlayer(int id)
    {
        var player = await _service.GetByIdAsync(id);
        return player is null ? NotFound() : Ok(player);
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var bestCountry = await _service.GetBestCountryByWinRatioAsync();
        var avgMC = await _service.GetAverageMCAsync();
        var medianHeight = await _service.GetMedianHeightAsync();

        return Ok(new
        {
            BestCountry = bestCountry,
            AverageMC = avgMC,
            MedianHeight = medianHeight
        });
    }

    [HttpPost]
    public async Task<IActionResult> AddPlayer([FromBody] Player player)
    {
        await _service.AddAsync(player);
        return CreatedAtAction(nameof(GetPlayer), new { id = player.id }, player);
    }
}