namespace PokeTactics.Contracts.Move.PokeApi;

public class MoveSummaryListPokeApiResponse
{
    public int Count { get; set; }

    public required ICollection<MoveSummaryPokeApiResponse> Results { get; set; }
}
