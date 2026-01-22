using PokeTactics.Contracts.Sprite.PokeApi;
using PokeTactics.Contracts.Sprite.Responses;
using PokeTactics.Core.Entities;

namespace PokeTactics.Services.Mappers;

public static class SpriteMapper
{
    // DTO -> Entity
    public static Sprite ToSprite(this SpritePokeApiResponse spritePokeApiResponse)
    {
        return new Sprite
        {
            BackFemaleUri = spritePokeApiResponse.BackFemaleUri,
            BackMaleUri = spritePokeApiResponse.BackMaleUri,
            BackShinyFemaleUri = spritePokeApiResponse.BackShinyFemaleUri,
            BackShinyMaleUri = spritePokeApiResponse.BackShinyMaleUri,
            FrontFemaleUri = spritePokeApiResponse.FrontFemaleUri,
            FrontMaleUri = spritePokeApiResponse.FrontMaleUri,
            FrontShinyFemaleUri = spritePokeApiResponse.FrontShinyFemaleUri,
            FrontShinyMaleUri = spritePokeApiResponse.FrontShinyMaleUri,
            OfficialArtworkUri = spritePokeApiResponse.OtherSprites.OfficialArtworkSprite.FrontMaleUri
        };
    }

    // Entity -> DTO
    public static SpriteDto ToSpriteDto(this Sprite sprite)
    {
        return new SpriteDto
        {
            BackFemaleUri = sprite.BackFemaleUri,
            BackMaleUri = sprite.BackMaleUri,
            BackShinyFemaleUri = sprite.BackShinyFemaleUri,
            BackShinyMaleUri = sprite.BackShinyMaleUri,
            FrontFemaleUri = sprite.FrontFemaleUri,
            FrontMaleUri = sprite.FrontMaleUri,
            FrontShinyFemaleUri = sprite.FrontShinyFemaleUri,
            FrontShinyMaleUri = sprite.FrontShinyMaleUri,
            OfficialArtworkUri = sprite.OfficialArtworkUri
        };
    }
}
