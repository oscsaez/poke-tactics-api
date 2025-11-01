using PokeTactics.Contracts.Ability.PokeApi;
using PokeTactics.Contracts.Utils.Extensions;
using PokeTactics.Core.Entities;

namespace PokeTactics.Services.Mappers;

// TODO: Change all these mappers that receive more parameters that the one to be mapped and maybe use builder pattern?
public static class AbilityMapper
{
    // DTO -> Entity
    public static Ability ToAbility(this AbilityEffectPokeApiResponse abilityEffectPokeApiResponse, string name)
    {
        return new Ability
        {
            Description = abilityEffectPokeApiResponse.EffectEntries.TakeEnglishToString(),
            Name = name
        };
    }

    // Entity -> Entity
    public static void MapExisting(this Ability trackedAbility, Ability nonTrackedAbility)
    {
        trackedAbility.Name = nonTrackedAbility.Name;
        trackedAbility.Description = nonTrackedAbility.Description;
    }
}
