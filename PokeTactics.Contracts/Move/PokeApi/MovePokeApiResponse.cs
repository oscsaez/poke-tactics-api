using System.Text.Json.Serialization;

namespace PokeTactics.Contracts.Move.PokeApi
{
    public class MovePokeApiResponse
    {
        [JsonPropertyName("move")]
        public required MoveUriPokeApiResponse MoveUriPokeApiResponse { get; set; }
    }
}