namespace PokeTactics.Contracts.Pokemon.PokeApi
{
    public class PokemonSummaryListPokeApiResponse
    {
        public int Count { get; set; }

        public required ICollection<PokemonSummaryPokeApiResponse> Results { get; set; }
    }
}