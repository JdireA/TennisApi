using System.Text.Json;
using TennisApi.Models;

namespace TennisApi.Services;

public sealed class PlayerService : IPlayerService
{
    private readonly List<Player> _players;

    public PlayerService(IWebHostEnvironment env)
    {
        var jsonPath = Path.Combine(env.ContentRootPath, "Data", "headtohead.json");
        var json = File.ReadAllText(jsonPath);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true  // Important pour gérer firstname/FirstName
        };
        var response = JsonSerializer.Deserialize<PlayersResponse>(json, options)!;
        _players = response.Players;
    }

    public Task<IEnumerable<Player>> GetAllAsync() =>
        Task.FromResult(_players.OrderBy(p => p.data.rank).AsEnumerable());
        //Task.FromResult(_players.AsEnumerable());

    public Task<Player?> GetByIdAsync(int id) =>
        Task.FromResult(_players.FirstOrDefault(p => p.id == id));

    public Task AddAsync(Player player)
    {
        _players.Add(player);
        return Task.CompletedTask;
    }

    public Task<double> GetAverageMCAsync() =>
        Task.FromResult(_players.Average(p => p.data.last.Count(x => x == 1)));

    public Task<double> GetMedianHeightAsync()
    {
        var heights = _players.Select(p => p.data.height).OrderBy(h => h).ToList();
        int count = heights.Count;

        double median = count % 2 == 1
            ? heights[count / 2]
            : (heights[count / 2 - 1] + heights[count / 2]) / 2.0;

        return Task.FromResult(median);
    }

    public Task<string> GetBestCountryByWinRatioAsync()
    {
        var result = _players
            .GroupBy(p => p.country.code)
            .Select(g => new
            {
                Country = g.Key,
                Ratio = g.Average(p => p.data.last.Count(x => x == 1) / (double)p.data.last.Count)
            })
            .OrderByDescending(x => x.Ratio)
            .First()
            .Country;

        return Task.FromResult(result);
    }
}