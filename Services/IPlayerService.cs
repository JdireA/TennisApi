
public interface IPlayerService
{
    Task<IEnumerable<Player>> GetAllPlayersAsync();
    Task<Player?> GetPlayerByIdAsync(int id);
    Task<StatisticsResponse> GetStatisticsAsync();
    Task<Player> AddPlayerAsync(CreatePlayerRequest newPlayer);
}