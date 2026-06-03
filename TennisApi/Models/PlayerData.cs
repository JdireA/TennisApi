namespace TennisApi.Models
{
    public sealed record PlayerData(
        int rank,
        int points,
        int weight,   // en grammes
        int height,   // en cm
        int age,
        IReadOnlyList<int> last
    );
}
