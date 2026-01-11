using System.Text.Json.Serialization;

namespace PokeTactics.Contracts.Sprite.PokeApi;

public class SpritePokeApiResponse
{
    [JsonPropertyName("other")]
    public required OtherSpritesPokeApiResponse OtherSprites { get; set; }

    [JsonPropertyName("back_default")]
    public string? BackMaleUri { get; set; }

    [JsonPropertyName("back_female")]
    public string? BackFemaleUri { get; set; }

    [JsonPropertyName("back_shiny")]
    public string? BackShinyMaleUri { get; set; }

    [JsonPropertyName("back_shiny_female")]
    public string? BackShinyFemaleUri { get; set; }

    [JsonPropertyName("front_default")]
    public required string FrontMaleUri { get; set; }

    [JsonPropertyName("front_female")]
    public string? FrontFemaleUri { get; set; }

    [JsonPropertyName("front_shiny")]
    public string? FrontShinyMaleUri { get; set; }

    [JsonPropertyName("front_shiny_female")]
    public string? FrontShinyFemaleUri { get; set; }
}
