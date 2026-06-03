
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PlayersController : ControllerBase
{
    private readonly IPlayerService _playerService;

    public PlayersController(IPlayerService playerService)
    {
        _playerService = playerService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Player>>> GetPlayers()
    {
        var players = await _playerService.GetAllPlayersAsync();
        return Ok(players);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Player>> GetPlayer(int id)
    {
        var player = await _playerService.GetPlayerByIdAsync(id);
        return player is null 
            ? NotFound(new { message = $"Joueur avec l'ID {id} non trouvé" })
            : Ok(player);
    }

    [HttpGet("statistics")]
    public async Task<ActionResult<StatisticsResponse>> GetStatistics()
    {
        var stats = await _playerService.GetStatisticsAsync();
        return Ok(stats);
    }

    [HttpPost]
    public async Task<ActionResult<Player>> AddPlayer([FromBody] CreatePlayerRequest request)
    {
        // Validation basique
        if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
        {
            return BadRequest(new { error = "FirstName et LastName sont requis" });
        }

        if (request.Data.Rank <= 0 || request.Data.Points < 0)
        {
            return BadRequest(new { error = "Rank doit être positif et Points doit être >= 0" });
        }

        var newPlayer = await _playerService.AddPlayerAsync(request);
        
        return CreatedAtAction(nameof(GetPlayer), new { id = newPlayer.Id }, newPlayer);
    }
}