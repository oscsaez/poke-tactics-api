using PokeTactics.Core.Utils.Extensions;

namespace PokeTactics.Test.Core.Utils.Extensions;

public class StringExtensionsTest
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ToUri(bool isRelativeUri)
    {
        // Arrange
        string uriAsString = isRelativeUri ? "/uri" : "https://uri/";

        // Act
        Uri uri = isRelativeUri ? uriAsString.ToUri(UriKind.Relative) : uriAsString.ToUri();

        // Assert
        Assert.Equal(uriAsString, uri.ToString());
    }

    [Theory]
    [InlineData(new[] { "a", "b" }, new[] { "a", "b" }, true)]
    [InlineData(new[] { "a", "b" }, new[] { "b", "a" }, true)]
    [InlineData(new[] { "a", "b" }, new[] { "a" }, false)]
    [InlineData(new[] { "a", "b" }, new[] { "a", "c" }, false)]
    [InlineData(new string[] { }, new string[] { }, true)]
    [InlineData(new[] { "a", "a" }, new[] { "a", "b" }, false)]
    public void CompareStringCollections(string[] collection1, string[] collection2, bool expected)
    {
        // Act
        bool result = collection1.Compare(collection2);

        // Assert
        Assert.Equal(expected, result);
    }
}
