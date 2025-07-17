using System.Text.Json.Serialization;

namespace PokeTactics.Contracts.Sprite.PokeApi;

public class OfficialArtworkSpritePokeApiResponse
{
    [JsonPropertyName("front_default")]
    public required string FrontMaleUri { get; set; }
}
