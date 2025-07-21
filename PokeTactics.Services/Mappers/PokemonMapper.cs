using PokeTactics.Contracts.Pokemon.PokeApi;
using PokeTactics.Core.Entities;

namespace PokeTactics.Services.Mappers;

public static class PokemonMapper
{
    public static Pokemon ToPokemon(this PokemonPokeApiResponse pokemonPokeApiResponse)
    {
        // Abilities and moves should be filled after mapping
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
}
