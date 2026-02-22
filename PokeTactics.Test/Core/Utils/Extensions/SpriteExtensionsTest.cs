using PokeTactics.Core.Entities;
using PokeTactics.Core.Utils.Extensions;

namespace PokeTactics.Test.Core.Utils.Extensions;

public class SpriteExtensionsTest
{
    [Fact]
    public void Compare_AllUrisAreIdentical_ReturnsTrue()
    {
        // Arrange
        var sprite1 = CreateValidSprite();
        var sprite2 = CreateValidSprite();

        // Act
        var result = sprite1.Compare(sprite2);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("OfficialArtworkUri")]
    [InlineData("FrontMaleUri")]
    [InlineData("BackMaleUri")]
    [InlineData("FrontShinyMaleUri")]
    [InlineData("BackShinyMaleUri")]
    [InlineData("FrontFemaleUri")]
    [InlineData("BackFemaleUri")]
    [InlineData("FrontShinyFemaleUri")]
    [InlineData("BackShinyFemaleUri")]
    public void Compare_AnyUriIsDifferent_ReturnsFalse(string propertyToChange)
    {
        // Arrange
        var sprite1 = CreateValidSprite();
        var sprite2 = CreateValidSprite();

        // Modificamos solo la propiedad que queremos testear
        typeof(Sprite).GetProperty(propertyToChange)!.SetValue(sprite2, "https://different-uri.com");

        // Act
        var result = sprite1.Compare(sprite2);

        // Assert
        Assert.False(result, $"Debería haber fallado al cambiar {propertyToChange}");
    }

    private static Sprite CreateValidSprite() => new()
    {
        OfficialArtworkUri = "https://art.com",
        FrontMaleUri = "https://fm.com",
        BackMaleUri = "https://bm.com",
        FrontShinyMaleUri = "https://fsm.com",
        BackShinyMaleUri = "https://bsm.com",
        FrontFemaleUri = "https://ff.com",
        BackFemaleUri = "https://bf.com",
        FrontShinyFemaleUri = "https://fsf.com",
        BackShinyFemaleUri = "https://bsf.com"
    };
}
