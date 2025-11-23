using PokeTactics.Contracts.Pokemon.PokeApi;
using PokeTactics.Core.Entities;

namespace PokeTactics.Services.Mappers;

public static class PokemonMapper
{
    // DTO -> Entity
    public static Pokemon ToPokemonWithAbilitiesAndMoves(this PokemonPokeApiResponse pokemonPokeApiResponse)
    {
        Pokemon pokemon = pokemonPokeApiResponse.ToPokemon();
        pokemon.AbilitiesInPokemon = pokemonPokeApiResponse.Abilities.ToAbilitiesInPokemon();
        pokemon.MovesInPokemon = pokemonPokeApiResponse.Moves.ToMovesInPokemon();

        return pokemon;
    }

    public static Pokemon ToPokemon(this PokemonPokeApiResponse pokemonPokeApiResponse)
    {
        return new Pokemon
        {
            PokedexOrder = pokemonPokeApiResponse.Order,
            Name = pokemonPokeApiResponse.Name,
            Height = pokemonPokeApiResponse.Height,
            Weight = pokemonPokeApiResponse.Weight,
            Types = [.. pokemonPokeApiResponse.Types.Select(x => x.TypeInfoPokeApiResponse.Name)],
            Sprite = pokemonPokeApiResponse.Sprite.ToSprite(),
            Stats = pokemonPokeApiResponse.Stats.ToStats()
        };
    }

    // Entity -> Entity
    public static void MapExisting(
        this Pokemon trackedPokemon,
        Pokemon nonTrackedPokemon,
        IDictionary<string, Ability> abilitiesMap,
        IDictionary<string, Move> movesMap)
    {
        trackedPokemon.PokedexOrder = nonTrackedPokemon.PokedexOrder;
        trackedPokemon.Height = nonTrackedPokemon.Height;
        trackedPokemon.Weight = nonTrackedPokemon.Weight;
        trackedPokemon.Types = nonTrackedPokemon.Types;
        trackedPokemon.Stats = nonTrackedPokemon.Stats;
        trackedPokemon.Sprite = nonTrackedPokemon.Sprite;
        trackedPokemon.MapAbilities(nonTrackedPokemon, abilitiesMap);
        trackedPokemon.MapMoves(nonTrackedPokemon, movesMap);
    }

    private static void MapAbilities(this Pokemon trackedPokemon, Pokemon nonTrackedPokemon, IDictionary<string, Ability> abilitiesMap)
    {
        IEnumerable<string> trackedAbilityNames = trackedPokemon.AbilitiesInPokemon.Select(x => x.Ability.Name);

        foreach (AbilityInPokemon nonTrackedAbilityInPokemon in nonTrackedPokemon.AbilitiesInPokemon)
        {
            if (!trackedAbilityNames.Contains(nonTrackedAbilityInPokemon.Ability.Name) &&
                abilitiesMap.TryGetValue(nonTrackedAbilityInPokemon.Ability.Name, out Ability ability))
            {
                AbilityInPokemon newAbilityInPokemon = new()
                {
                    AbilityId = ability.Id,
                    IsHidden = nonTrackedAbilityInPokemon.IsHidden
                };

                trackedPokemon.AbilitiesInPokemon.Add(newAbilityInPokemon);
            }
        }
    }

    private static void MapMoves(this Pokemon trackedPokemon, Pokemon nonTrackedPokemon, IDictionary<string, Move> movesMap)
    {
        IEnumerable<string> trackedMoveNames = trackedPokemon.MovesInPokemon.Select(x => x.Move.Name);

        foreach (MoveInPokemon nonTrackedMoveInPokemon in nonTrackedPokemon.MovesInPokemon)
        {
            if (!trackedMoveNames.Contains(nonTrackedMoveInPokemon.Move.Name) &&
                movesMap.TryGetValue(nonTrackedMoveInPokemon.Move.Name, out Move move))
            {
                MoveInPokemon moveInPokemon = new()
                {
                    MoveId = move.Id
                };

                trackedPokemon.MovesInPokemon.Add(moveInPokemon);
            }
        }
    }
}
