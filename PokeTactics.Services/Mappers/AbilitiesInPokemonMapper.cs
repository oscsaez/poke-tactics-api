using PokeTactics.Contracts.Ability.PokeApi;
using PokeTactics.Core.Entities;

namespace PokeTactics.Services.Mappers;

public static class AbilitiesInPokemonMapper
{
    public static AbilitiesInPokemon AbilityPokeApiResponseToAbilitiesInPokemon(AbilitySlotPokeApiResponse abilitySlotPokeApiResponse, AbilityEffectPokeApiResponse abilityEffectPokeApiResponse)
    {
        return new AbilitiesInPokemon
        {
            Ability = AbilityMapper.AbilityPokeApiResponseToAbility(abilitySlotPokeApiResponse, abilityEffectPokeApiResponse)
        };
    }
}
