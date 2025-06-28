namespace PokeTactics.Api.Utils
{
    public static class ApiConstants
    {
        public const string ContentType = "application/json";
        public const string DefaultConnection = "DefaultConnection";
        public const string MigrationsAssembly = "PokeTactics.Infrastructure";

        // HostedService
        public const string BasePokemonInfoUri = "https://pokeapi.co/api/v2/";
        public static readonly string AllPokemonInfoUri = $"{BasePokemonInfoUri}pokemon/?limit={int.MaxValue}";
    }
}