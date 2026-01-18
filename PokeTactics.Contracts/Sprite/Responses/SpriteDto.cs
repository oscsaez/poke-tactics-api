using System;

namespace PokeTactics.Contracts.Sprite.Responses;

public class SpriteDto
{
    public string? OfficialArtworkUri { get; set; }

    public string? BackMaleUri { get; set; }

    public string? BackFemaleUri { get; set; }

    public string? BackShinyMaleUri { get; set; }

    public string? BackShinyFemaleUri { get; set; }

    public string? FrontMaleUri { get; set; }

    public string? FrontFemaleUri { get; set; }

    public string? FrontShinyMaleUri { get; set; }

    public string? FrontShinyFemaleUri { get; set; }
}
