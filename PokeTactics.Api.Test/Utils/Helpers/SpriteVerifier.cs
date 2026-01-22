using PokeTactics.Contracts.Sprite.PokeApi;
using PokeTactics.Contracts.Sprite.Responses;
using PokeTactics.Core.Entities;

namespace PokeTactics.Api.Test.Utils.Helpers;

public static class SpriteVerifier
{
    public static void VerifySprite(SpritePokeApiResponse expected, Sprite actual)
    {
        Assert.Equal(expected.BackFemaleUri, actual.BackFemaleUri);
        Assert.Equal(expected.BackMaleUri, actual.BackMaleUri);
        Assert.Equal(expected.BackShinyFemaleUri, actual.BackShinyFemaleUri);
        Assert.Equal(expected.BackShinyMaleUri, actual.BackShinyMaleUri);
        Assert.Equal(expected.FrontFemaleUri, actual.FrontFemaleUri);
        Assert.Equal(expected.FrontMaleUri, actual.FrontMaleUri);
        Assert.Equal(expected.FrontShinyFemaleUri, actual.FrontShinyFemaleUri);
        Assert.Equal(expected.FrontShinyMaleUri, actual.FrontShinyMaleUri);
        Assert.Equal(expected.OtherSprites.OfficialArtworkSprite.FrontMaleUri, actual.OfficialArtworkUri);
    }

    public static void VerifySpriteDto(Sprite expected, SpriteDto actual)
    {
        Assert.Equal(expected.BackFemaleUri, actual.BackFemaleUri);
        Assert.Equal(expected.BackMaleUri, actual.BackMaleUri);
        Assert.Equal(expected.BackShinyFemaleUri, actual.BackShinyFemaleUri);
        Assert.Equal(expected.BackShinyMaleUri, actual.BackShinyMaleUri);
        Assert.Equal(expected.FrontFemaleUri, actual.FrontFemaleUri);
        Assert.Equal(expected.FrontMaleUri, actual.FrontMaleUri);
        Assert.Equal(expected.FrontShinyFemaleUri, actual.FrontShinyFemaleUri);
        Assert.Equal(expected.FrontShinyMaleUri, actual.FrontShinyMaleUri);
        Assert.Equal(expected.OfficialArtworkUri, actual.OfficialArtworkUri);
    }
}
