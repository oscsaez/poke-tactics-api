namespace PokeTactics.Contracts.Stat.PokeApi
{
    public class StatPokeApiResponse
    {
        public required StatInfoPokeApiResponse Stat { get; set; }

        public int Base { get; set; }
    }
}