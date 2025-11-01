using PokeTactics.Contracts.Ability.PokeApi;
using PokeTactics.Core.Entities;

namespace PokeTactics.Services.Mappers;

public static class AbilitiesInPokemonMapper
{
    public static AbilitiesInPokemon AbilityPokeApiResponseToAbilitiesInPokemon(AbilitySlotPokeApiResponse abilitySlotPokeApiResponse, AbilityEffectPokeApiResponse abilityEffectPokeApiResponse)
    {
        return new AbilitiesInPokemon
        {
            // TODO: Change this when creting/updating pokemon and their relationships with abilities
            Ability = abilityEffectPokeApiResponse.ToAbility(abilitySlotPokeApiResponse.AbilityInfo.Name),
            IsHidden = abilitySlotPokeApiResponse.IsHidden
        };
    }
}
