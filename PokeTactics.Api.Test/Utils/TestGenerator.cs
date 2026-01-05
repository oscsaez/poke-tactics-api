namespace PokeTactics.Api.Test.Utils;

public static class TestGenerator
{
    private const int DefaultRandomMin = 0;
    private const int DefaultRandomMax = 100;
    private const int MaxNameLength = 8;

    public static string RandomGuidAsString() => Guid.NewGuid().ToString();

    public static string RandomPath() => $"/{RandomGuidAsString()}";

    public static string RandomName() => RandomGuidAsString()[..MaxNameLength];

    public static int RandomInt() => RandomInt(DefaultRandomMin, DefaultRandomMax);

    public static int RandomInt(int min, int max) => Random.Shared.Next(min, max);
}
