namespace TennisApi.Models
{
    public sealed record Player(
        int id,
        string firstname,
        string lastname,
        string shortname,
        string sex,
        Country country,
        string picture,
        PlayerData data
    );
}
