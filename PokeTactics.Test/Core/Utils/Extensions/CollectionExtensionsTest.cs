using PokeTactics.Core.Utils.Extensions;

namespace PokeTactics.Test.Core.Utils.Extensions;

public class CollectionExtensionsTest
{
    [Fact]
    public void AddRange_AddsEveryElementOfOneCollectionToAnother()
    {
        // Arrange
        ICollection<string> firstList = ["a", "b", "c"];
        ICollection<string> secondList = ["d", "e", "f"];
        ICollection<string> firstListCopy = [.. firstList];

        // Act
        firstList.AddRange(secondList);

        // Assert
        Assert.All(firstList, x => Assert.True(firstListCopy.Contains(x) || secondList.Contains(x)));
    }
}
