using TennisApi.Models;

namespace TennisApi.Services;

public interface IPlayerService
{
    Task<IEnumerable<Player>> GetAllAsync();
    Task<Player?> GetByIdAsync(int id);
    Task AddAsync(Player player);

    Task<double> GetAverageMCAsync();
    Task<double> GetMedianHeightAsync();
    Task<string> GetBestCountryByWinRatioAsync();
}