
public record CreatePlayerRequest(
    string FirstName,
    string LastName,
    string ShortName,
    string Sex,
    Country Country,
    PlayerData Data
);