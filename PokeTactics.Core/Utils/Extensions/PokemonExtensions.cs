using PokeTactics.Core.Entities;

namespace PokeTactics.Core.Utils.Extensions;

public static class PokemonExtensions
{
    public static bool Compare(this Pokemon pokemon1, Pokemon pokemon2)
    {
        return pokemon1.CompareBaseTypeProperties(pokemon2) &&
            pokemon1.Types.SequenceEqual(pokemon2.Types) &&
            pokemon1.CompareStatsAndSprite(pokemon2) &&
            pokemon1.CompareAbilitiesAndMoves(pokemon2);
    }

    private static bool CompareBaseTypeProperties(this Pokemon pokemon1, Pokemon pokemon2)
    {
        return pokemon1.PokedexOrder == pokemon2.PokedexOrder &&
            pokemon1.Name == pokemon2.Name &&
            pokemon1.Height == pokemon2.Height &&
            pokemon1.Weight == pokemon2.Weight;
    }

    private static bool CompareStatsAndSprite(this Pokemon pokemon1, Pokemon pokemon2)
    {
        return pokemon1.Stats.Compare(pokemon2.Stats) &&
            pokemon1.Sprite.Compare(pokemon2.Sprite);
    }

    private static bool CompareAbilitiesAndMoves(this Pokemon pokemon1, Pokemon pokemon2)
    {
        return pokemon1.CompareAbilityNames(pokemon2) &&
            pokemon1.CompareMoveNames(pokemon2);
    }

    private static bool CompareAbilityNames(this Pokemon pokemon1, Pokemon pokemon2)
    {
        List<string> abilityNames1 = [.. pokemon1.AbilitiesInPokemon.Select(x => x.Ability.Name)];
        List<string> abilityNames2 = [.. pokemon2.AbilitiesInPokemon.Select(x => x.Ability.Name)];

        return abilityNames1.Compare(abilityNames2);
    }

    private static bool CompareMoveNames(this Pokemon pokemon1, Pokemon pokemon2)
    {
        List<string> moveNames1 = [.. pokemon1.MovesInPokemon.Select(x => x.Move.Name)];
        List<string> moveNames2 = [.. pokemon2.MovesInPokemon.Select(x => x.Move.Name)];

        return moveNames1.Compare(moveNames2);
    }
}
