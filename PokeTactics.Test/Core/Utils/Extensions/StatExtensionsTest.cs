using PokeTactics.Core.Entities;
using PokeTactics.Core.Utils.Extensions;

namespace PokeTactics.Test.Core.Utils.Extensions;

public class StatExtensionsTest
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Compare(bool areEqual)
    {
        // Arrange
        Stat stat1 = BuildStat();
        Stat stat2 = BuildStat(areEqual);

        // Act
        bool result = stat1.Compare(stat2);

        // Assert
        Assert.Equal(areEqual, result);
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, true)]
    [InlineData(true, false)]
    public void CompareCollections(bool areEqual, bool sameLength)
    {
        // Arrange
        Stat stat1 = BuildStat();
        Stat stat2 = BuildStat(areEqual);

        ICollection<Stat> stats1 = [stat1];
        ICollection<Stat> stats2 = [stat2];

        if (!sameLength)
        {
            stats2.Add(stat1);
        }

        // Act
        bool result = stats1.Compare(stats2);

        // Assert
        Assert.Equal(areEqual && sameLength, result);
    }

    private static Stat BuildStat(bool areEqual)
    {
        if (areEqual)
        {
            return BuildStat();
        }

        return BuildStat("RandomName", 1);
    }

    private static Stat BuildStat()
    {
        const string Name = "Name";
        const int Zero = 0;

        return BuildStat(Name, Zero);
    }

    private static Stat BuildStat(string name, int baseStat)
    {
        return new Stat
        {
            Name = name,
            Base = baseStat
        };
    }
}
