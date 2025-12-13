using System.Text.Json.Serialization;

namespace PokeTactics.Contracts.Sprite.PokeApi;

public class OtherSpritesPokeApiResponse
{
    [JsonPropertyName("official-artwork")]
    public required OfficialArtworkSpritePokeApiResponse OfficialArtworkSprite { get; set; }
}
