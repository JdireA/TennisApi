
public record Player(
    int Id,
    string FirstName,
    string LastName,
    string ShortName,
    string Sex,
    Country Country,
    PlayerData Data,
    Dictionary<string, int> Last
);