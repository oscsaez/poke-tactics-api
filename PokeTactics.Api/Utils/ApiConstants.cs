namespace PokeTactics.Api.Utils
{
    public static class ApiConstants
    {
        public const string ContentType = "application/json";
        public const string DefaultConnection = "DefaultConnection";
        public const string MigrationsAssembly = "PokeTactics.Infrastructure";

        // External API
        public const string ExternalApiName = "PokeApi";

        // HostedService
        public const string BasePokemonInfoUri = "https://pokeapi.co/api/v2/";
        public static readonly string AllAbilitiesInfoUri = $"ability/?limit={int.MaxValue}";
        public static readonly string AllPokemonInfoUri = $"pokemon/?limit={int.MaxValue}";
        public static readonly string AllMovesInfoUri = $"move/?limit={int.MaxValue}";

        // SyncServices
        public const int NumberOfRequestsGrantedConcurrently = 5;
    }
}