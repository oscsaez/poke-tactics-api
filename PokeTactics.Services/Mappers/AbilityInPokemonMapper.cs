using PokeTactics.Contracts.Ability.PokeApi;
using PokeTactics.Core.Entities;

namespace PokeTactics.Services.Mappers;

public static class AbilityInPokemonMapper
{
    public static AbilityInPokemon ToAbilityInPokemon(this AbilitySlotPokeApiResponse abilitySlotPokeApiResponse)
    {
        return new AbilityInPokemon
        {
            Ability = abilitySlotPokeApiResponse.ToAbility(),
            IsHidden = abilitySlotPokeApiResponse.IsHidden
        };
    }

    public static ICollection<AbilityInPokemon> ToAbilitiesInPokemon(this ICollection<AbilitySlotPokeApiResponse> abilitySlotPokeApiResponses)
    {
        return [.. abilitySlotPokeApiResponses.Select(ToAbilityInPokemon)];
    }
}
