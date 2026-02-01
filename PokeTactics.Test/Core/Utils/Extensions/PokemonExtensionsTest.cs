using PokeTactics.Core.Entities;
using PokeTactics.Core.Utils.Extensions;

namespace PokeTactics.Test.Core.Utils.Extensions;

public class PokemonExtensionsTest
{
    [Fact]
    public void Compare_PokemonsAreIdentical_ReturnsTrue()
    {
        // Arrange
        Pokemon pokemon1 = CreateBasePokemon();
        Pokemon pokemon2 = CreateBasePokemon();

        // Act
        bool result = pokemon1.Compare(pokemon2);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [MemberData(nameof(GetPokemonDifferences))]
    public void Compare_ShouldReturnFalse_WhenThereIsAnyDifference(Pokemon pokemon2, string scenario)
    {
        // Arrange
        Pokemon pokemon1 = CreateBasePokemon();

        // Act
        bool result = pokemon1.Compare(pokemon2);

        // Assert
        Assert.False(result, $"Failed scenario: {scenario}");
    }

    public static IEnumerable<object[]> GetPokemonDifferences()
    {
        Pokemon pokemonWithDifferentName = CreateBasePokemon();
        pokemonWithDifferentName.Name = "Pikachu";
        yield return new object[] { pokemonWithDifferentName, "Different name" };

        Pokemon pokemonWithDifferentWeight = CreateBasePokemon(); 
        pokemonWithDifferentWeight.Weight = 999;
        yield return new object[] { pokemonWithDifferentWeight, "Different weight" };

        Pokemon pokemonWithDifferentTypes = CreateBasePokemon(); 
        pokemonWithDifferentTypes.Types = ["Electric"];
        yield return new object[] { pokemonWithDifferentTypes, "Different types (order or content)" };

        Pokemon pokemonWithDifferentAbilities = CreateBasePokemon();
        pokemonWithDifferentAbilities.AbilitiesInPokemon = [
            new AbilityInPokemon 
            { 
                Ability = new Ability 
                { 
                    Name = "Static" 
                } 
            }
        ];
        yield return new object[] { pokemonWithDifferentAbilities, "Different abilities" };

        Pokemon pokemonWithDifferentStats = CreateBasePokemon(); 
        pokemonWithDifferentStats.Stats.First().Base = 20;
        yield return new object[] { pokemonWithDifferentStats, "Different stats" };
    }

    private static Pokemon CreateBasePokemon()
    {
        return new Pokemon
        {
            PokedexOrder = 1,
            Name = "Bulbasaur",
            Height = 7,
            Weight = 69,
            Types = ["Grass", "Poison"],
            Stats = [
                new Stat 
                { 
                    Name = "Hp", 
                    Base = 10 
                }
            ],
            Sprite = new Sprite 
            { 
                OfficialArtworkUri = "http://uri.com" 
            },
            AbilitiesInPokemon = [
                new AbilityInPokemon 
                { 
                    Ability = new Ability 
                    { 
                        Name = "Overgrow" 
                    } 
                }
            ],
            MovesInPokemon = [
                new MoveInPokemon 
                { 
                    Move = new Move 
                    { 
                        Name = "Tackle",
                        Type = "Normal" 
                    } 
                }
            ]
        };
    }
}
