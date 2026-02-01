using PokeTactics.Core.Entities;

namespace PokeTactics.Core.Utils.Extensions;

public static class SpriteExtensions
{
    public static bool Compare(this Sprite sprite1, Sprite sprite2)
    {
        return sprite1.OfficialArtworkUri == sprite2.OfficialArtworkUri &&
            sprite1.CompareMaleUris(sprite2) &&
            sprite1.CompareFemaleUris(sprite2);
    }

    private static bool CompareMaleUris(this Sprite sprite1, Sprite sprite2)
    {
        return sprite1.BackMaleUri == sprite2.BackMaleUri &&
            sprite1.BackShinyMaleUri == sprite2.BackShinyMaleUri &&
            sprite1.FrontMaleUri == sprite2.FrontMaleUri &&
            sprite1.FrontShinyMaleUri == sprite2.FrontShinyMaleUri;
    }
    
    private static bool CompareFemaleUris(this Sprite sprite1, Sprite sprite2)
    {
        return sprite1.BackFemaleUri == sprite2.BackFemaleUri &&
            sprite1.BackShinyFemaleUri == sprite2.BackShinyFemaleUri &&
            sprite1.FrontFemaleUri == sprite2.FrontFemaleUri &&
            sprite1.FrontShinyFemaleUri == sprite2.FrontShinyFemaleUri;
    }
}
