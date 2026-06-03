using TennisApi.Models;

namespace TennisApi.Tests;

public static class TestDataFactory
{
    public static List<Player> CreatePlayers() =>
        new()
        {
            new Player(
                id: 1,
                firstname: "Novak",
                lastname: "Djokovic",
                shortname: "N.DJO",
                sex: "M",
                country: new Country("pic1", "SRB"),
                picture: "pic1",
                data: new PlayerData(
                    rank: 2,
                    points: 2500,
                    weight: 80000,
                    height: 188,
                    age: 36,
                    last: new List<int> { 1, 1, 1, 1, 1 }
                )
            ),
            new Player(
                id: 2,
                firstname: "Rafael",
                lastname: "Nadal",
                shortname: "R.NAD",
                sex: "M",
                country: new Country("pic2", "ESP"),
                picture: "pic2",
                data: new PlayerData(
                    rank: 5,
                    points: 2000,
                    weight: 85000,
                    height: 185,
                    age: 37,
                    last: new List<int> { 1, 0, 1, 0, 1 }
                )
            )
        };
}