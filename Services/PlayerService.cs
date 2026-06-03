using System.Text.Json;

public class PlayerService : IPlayerService
{
    private readonly string _jsonPath;
    private readonly ILogger<PlayerService> _logger;

    public PlayerService(IWebHostEnvironment env, ILogger<PlayerService> logger)
    {
        _jsonPath = Path.Combine(env.ContentRootPath, "Data", "headtohead.json");
        _logger = logger;
    }

    public async Task<IEnumerable<Player>> GetAllPlayersAsync()
    {
        var players = await LoadPlayersAsync();
        return players.OrderBy(p => p.Data.Rank);
    }

    public async Task<Player?> GetPlayerByIdAsync(int id)
    {
        var players = await LoadPlayersAsync();
        return players.FirstOrDefault(p => p.Id == id);
    }

    public async Task<StatisticsResponse> GetStatisticsAsync()
    {
        var players = await LoadPlayersAsync();
        
        // 1. Pays avec le meilleur ratio de victoires
        var countryStats = players
            .GroupBy(p => p.Country.Code)
            .Select(g => new
            {
                CountryCode = g.Key,
                WinRatio = CalculateWinRatio(g.ToList())
            })
            .Where(x => x.WinRatio > 0)
            .MaxBy(x => x.WinRatio);

        // 2. IMC moyen
        var bmiAverage = players
            .Where(p => p.Data.Height > 0 && p.Data.Weight > 0)
            .Select(p => (p.Data.Weight / 1000.0) / Math.Pow(p.Data.Height / 100.0, 2))
            .DefaultIfEmpty(0)
            .Average();

        // 3. Médiane taille
        var heights = players
            .Where(p => p.Data.Height > 0)
            .Select(p => p.Data.Height)
            .OrderBy(h => h)
            .ToList();
        
        double medianHeight = CalculateMedian(heights);

        return new StatisticsResponse(
            BestCountryWinRatio: countryStats?.CountryCode ?? "N/A",
            WinRatioValue: Math.Round(countryStats?.WinRatio ?? 0, 3),
            AverageBMI: Math.Round(bmiAverage, 2),
            MedianHeight: Math.Round(medianHeight, 1)
        );
    }

    private double CalculateWinRatio(List<Player> players)
    {
        // Une victoire = valeur 1 dans le dictionnaire Last
        var totalWins = players.Sum(p => p.Last.Values.Count(v => v == 1));
        var totalMatches = players.Sum(p => p.Last.Count);
        
        return totalMatches == 0 ? 0 : (double)totalWins / totalMatches;
    }

    private double CalculateMedian(List<int> sortedValues)
    {
        int count = sortedValues.Count;
        if (count == 0) return 0;
        
        if (count % 2 == 1)
            return sortedValues[count / 2];
        else
            return (sortedValues[count / 2 - 1] + sortedValues[count / 2]) / 2.0;
    }

    public async Task<Player> AddPlayerAsync(CreatePlayerRequest request)
    {
        var players = await LoadPlayersAsync();
        
        // Générer nouvel ID
        int newId = players.Any() ? players.Max(p => p.Id) + 1 : 1;
        
        // Créer le nouveau joueur (record avec with expression)
        var newPlayer = new Player(
            Id: newId,
            FirstName: request.FirstName,
            LastName: request.LastName,
            ShortName: request.ShortName,
            Sex: request.Sex,
            Country: request.Country,
            Data: request.Data,
            Last: new Dictionary<string, int>() // Initialiser les stats de matchs
        );
        
        players.Add(newPlayer);
        await SavePlayersAsync(players);
        
        _logger.LogInformation("Nouveau joueur ajouté : {FirstName} {LastName} (ID: {Id})", 
            newPlayer.FirstName, newPlayer.LastName, newPlayer.Id);
        
        return newPlayer;
    }

    private async Task<List<Player>> LoadPlayersAsync()
    {
        try
        {
            var json = await File.ReadAllTextAsync(_jsonPath);
            var options = new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true
            };
            
            // Le JSON a une structure avec clés numériques ("52", "95", ...)
            var playerDict = JsonSerializer.Deserialize<Dictionary<string, Player>>(json, options);
            return playerDict?.Values.ToList() ?? new List<Player>();
        }
        catch (FileNotFoundException)
        {
            _logger.LogWarning("Fichier players.json non trouvé, initialisation avec une liste vide");
            return new List<Player>();
        }
    }

    private async Task SavePlayersAsync(List<Player> players)
    {
        // Convertir en dictionnaire pour garder le format original du JSON
        var playerDict = players.ToDictionary(p => p.Id.ToString());
        var json = JsonSerializer.Serialize(playerDict, new JsonSerializerOptions 
        { 
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
        await File.WriteAllTextAsync(_jsonPath, json);
    }
}