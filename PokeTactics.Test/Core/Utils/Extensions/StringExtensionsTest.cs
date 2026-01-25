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
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public void CompareStringCollections(bool areEqual, bool haveSameLength)
    {
        // Arrange
        ICollection<string> collection1 = ["a", "b"];
        ICollection<string> collection2 = ["a", "b"];

        if (!areEqual)
        {
            if (haveSameLength)
            {
                collection2.Remove("b");
                collection2.Add("c");
            }
            else
            {
                collection2.Add("b");
            }
        }

        // Act
        bool result = collection1.Compare(collection2);

        // Assert
        if (areEqual)
        {
            Assert.True(result);
        }
        else
        {
            Assert.False(result);
        }
    }
}
