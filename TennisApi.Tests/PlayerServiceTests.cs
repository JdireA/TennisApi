using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Moq;
using System.Diagnostics.Metrics;
using System.Numerics;
using TennisApi.Models;
using TennisApi.Services;

namespace TennisApi.Tests;

public class PlayerServiceTests
{
    private PlayerService CreateServiceWithData(List<Player> players)
    {
        var env = new Mock<IWebHostEnvironment>();
        env.Setup(e => e.ContentRootPath).Returns(Directory.GetCurrentDirectory());

        // On bypass la lecture JSON en injectant directement les données
        var service = (PlayerService)Activator.CreateInstance(
            typeof(PlayerService),
            args: new object[] { env.Object }
        )!;

        // On remplace la liste interne via réflexion
        typeof(PlayerService)
            .GetField("_players", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .SetValue(service, players);

        return service;
    }

    private PlayerService CreateService()
    {
        var env = new Mock<IWebHostEnvironment>();
        env.Setup(e => e.ContentRootPath).Returns(Directory.GetCurrentDirectory());

        // On bypass la lecture JSON en injectant directement les données
        var service = (PlayerService)Activator.CreateInstance(
            typeof(PlayerService),
            args: new object[] { env.Object }
        )!;

        return service;
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPlayersOrderedByRank()
    {
        var players = TestDataFactory.CreatePlayers();
        var service = CreateServiceWithData(players);

        var result = await service.GetAllAsync();

        result.Should().BeInAscendingOrder(p => p.data.rank);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectPlayer()
    {
        var players = TestDataFactory.CreatePlayers();
        var service = CreateServiceWithData(players);

        var result = await service.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.firstname.Should().Be("Novak");
    }

    [Fact]
    public async Task AddAsync_ShouldAddPlayer()
    {
        var players = TestDataFactory.CreatePlayers();
        var service = CreateServiceWithData(players);

        var newPlayer = new Player(
            99, "Test", "Player", "T.PLA", "M",
            new Country("pic", "FRA"), "pic", new PlayerData(10, 1000, 70000, 180, 25, new List<int> { 1, 0 })
        );

        await service.AddAsync(newPlayer);

        var result = await service.GetByIdAsync(99);
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAverageMCAsync_ShouldReturnCorrectAverage()
    {
        var players = TestDataFactory.CreatePlayers();
        var service = CreateServiceWithData(players);

        var result = await service.GetAverageMCAsync();

        result.Should().Be(4); // Djokovic = 5 wins, Nadal = 3 wins → avg = 4
    }

    [Fact]
    public async Task GetMedianHeightAsync_ShouldReturnMedian()
    {
        var players = TestDataFactory.CreatePlayers();
        var service = CreateServiceWithData(players);

        var result = await service.GetMedianHeightAsync();

        result.Should().Be(186.5);
    }

    [Fact]
    public async Task GetBestCountryByWinRatioAsync_ShouldReturnCorrectCountry()
    {
        var players = TestDataFactory.CreatePlayers();
        var service = CreateServiceWithData(players);

        var result = await service.GetBestCountryByWinRatioAsync();

        result.Should().Be("SRB"); // Djokovic = 100% win ratio
    }
}