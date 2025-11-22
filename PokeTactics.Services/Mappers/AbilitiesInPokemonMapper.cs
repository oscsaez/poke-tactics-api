using PokeTactics.Contracts.Ability.PokeApi;
using PokeTactics.Core.Entities;

namespace PokeTactics.Services.Mappers;

public static class AbilitiesInPokemonMapper
{
    public static AbilitiesInPokemon ToAbilityInPokemon(this AbilitySlotPokeApiResponse abilitySlotPokeApiResponse)
    {
        return new AbilitiesInPokemon
        {
            Ability = abilitySlotPokeApiResponse.ToAbility(),
            IsHidden = abilitySlotPokeApiResponse.IsHidden
        };
    }

    public static ICollection<AbilitiesInPokemon> ToAbilitiesInPokemon(this ICollection<AbilitySlotPokeApiResponse> abilitySlotPokeApiResponses)
    {
        return [.. abilitySlotPokeApiResponses.Select(ToAbilityInPokemon)];
    }
}
