namespace PokeTactics.Contracts.Ability.PokeApi;

public class AbilitySummaryListPokeApiResponse
{
    public int Count { get; set; }

    public required ICollection<AbilitySummaryPokeApiResponse> Results { get; set; }
}
