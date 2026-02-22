using PokeTactics.Core.Entities;
using PokeTactics.Core.Utils.Extensions;

namespace PokeTactics.Test.Core.Utils.Extensions;

public class MoveExtensionsTest
{
    [Fact]
    public void Compare_ShouldReturnTrue_WhenMovesAreIdentical()
    {
        // Arrange
        var move1 = BuildMove();
        var move2 = BuildMove();

        // Act
        var result = move1.Compare(move2);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [MemberData(nameof(GetMovesToCompare))]
    public void Compare_ShouldReturnFalse_WhenAnyPropertyDiffers(Move move1, Move move2)
    {
        // Act
        bool result = move1.Compare(move2);

        // Assert
        Assert.False(result);
    }

    private static Move BuildMove() => new()
    {
        Name = "Flamethrower",
        Description = "Flamethrower description",
        Power = 90,
        PowerPoints = 15,
        Type = "Fire"
    };

    public static TheoryData<Move, Move> GetMovesToCompare()
    {
        var data = new TheoryData<Move, Move>
        {
            // Different name
            {
                BuildMove(),
                new Move
                {
                    Name = "Different",
                    Description = "Flamethrower description",
                    Power = 90,
                    PowerPoints = 15,
                    Type = "Fire"
                }
            },

            // Different description
            {
                BuildMove(),
                new Move
                {
                    Name = "Flamethrower",
                    Description = "Different",
                    Power = 90,
                    PowerPoints = 15,
                    Type = "Fire"
                }
            },

            // Different power
            {
                BuildMove(),
                new Move
                {
                    Name = "Flamethrower",
                    Description = "Flamethrower description",
                    Power = 80,
                    PowerPoints = 15,
                    Type = "Fire"
                }
            },

            // Different power points
            {
                BuildMove(),
                new Move
                {
                    Name = "Flamethrower",
                    Description = "Flamethrower description",
                    Power = 90,
                    PowerPoints = 10,
                    Type = "Fire"
                }
            },

            // Different type
            {
                BuildMove(),
                new Move
                {
                    Name = "Flamethrower",
                    Description = "Flamethrower description",
                    Power = 90,
                    PowerPoints = 15,
                    Type = "Water"
                }
            }
        };

        return data;
    }
}
