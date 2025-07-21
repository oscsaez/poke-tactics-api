using PokeTactics.Contracts.Ability.PokeApi;
using PokeTactics.Core.Entities;
using PokeTactics.Services.Constants;

namespace PokeTactics.Services.Mappers;

public static class AbilityMapper
{
    public static Ability AbilityPokeApiResponseToAbility(AbilitySlotPokeApiResponse abilitySlotPokeApiResponse, AbilityEffectPokeApiResponse abilityEffectPokeApiResponse)
    {
        return new Ability
        {
            Description = abilityEffectPokeApiResponse.EffectEntries.TakeEnglishToString(),
            IsHidden = abilitySlotPokeApiResponse.IsHidden,
            Name = abilitySlotPokeApiResponse.AbilityInfo.Name
        };
    }
}
