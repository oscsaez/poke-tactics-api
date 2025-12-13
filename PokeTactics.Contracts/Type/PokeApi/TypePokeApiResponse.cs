using System.Text.Json.Serialization;

namespace PokeTactics.Contracts.Type.PokeApi
{
    public class TypePokeApiResponse
    {
        [JsonPropertyName("type")]
        public required TypeInfoPokeApiResponse TypeInfoPokeApiResponse { get; set; }
    }
}