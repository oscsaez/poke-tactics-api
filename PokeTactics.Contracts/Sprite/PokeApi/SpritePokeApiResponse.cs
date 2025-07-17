using System.Text.Json.Serialization;

namespace PokeTactics.Contracts.Sprite.PokeApi;

public class SpritePokeApiResponse
{
    [JsonPropertyName("other")]
    public required OtherSpritesPokeApiResponse OtherSprites { get; set; }

    [JsonPropertyName("back_default")]
    public required string BackMaleUri { get; set; }

    [JsonPropertyName("back_female")]
    public required string BackFeMaleUri { get; set; }

    [JsonPropertyName("back_shiny")]
    public required string BackShinyMaleUri { get; set; }

    [JsonPropertyName("back_shiny_female")]
    public required string BackShinyFemaleUri { get; set; }

    [JsonPropertyName("front_default")]
    public required string FrontMaleUri { get; set; }

    [JsonPropertyName("front_female")]
    public required string FrontFeMaleUri { get; set; }

    [JsonPropertyName("front_shiny")]
    public required string FrontShinyMaleUri { get; set; }

    [JsonPropertyName("front_shiny_female")]
    public required string FrontShinyFemaleUri { get; set; }
}
