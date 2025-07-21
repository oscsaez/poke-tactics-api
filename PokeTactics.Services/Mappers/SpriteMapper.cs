using PokeTactics.Contracts.Sprite.PokeApi;
using PokeTactics.Core.Entities;

namespace PokeTactics.Services.Mappers;

public static class SpriteMapper
{
    public static Sprite ToSprite(this SpritePokeApiResponse spritePokeApiResponse)
    {
        return new Sprite
        {
            BackFemaleUri = spritePokeApiResponse.BackFeMaleUri,
            BackMaleUri = spritePokeApiResponse.BackMaleUri,
            BackShinyFemaleUri = spritePokeApiResponse.BackShinyFemaleUri,
            BackShinyMaleUri = spritePokeApiResponse.BackShinyMaleUri,
            FrontFemaleUri = spritePokeApiResponse.FrontFeMaleUri,
            FrontMaleUri = spritePokeApiResponse.FrontMaleUri,
            FrontShinyFemaleUri = spritePokeApiResponse.FrontShinyFemaleUri,
            FrontShinyMaleUri = spritePokeApiResponse.FrontShinyMaleUri,
            OfficialArtworkUri = spritePokeApiResponse.OtherSprites.OfficialArtworkSprite.FrontMaleUri
        };
    }
}
