using PokeTactics.Api.Test.Fixture;
using PokeTactics.Contracts.Ability.PokeApi;
using PokeTactics.Contracts.Ability.Responses;
using PokeTactics.Core.Entities;
using PokeTactics.Core.Interfaces;

namespace PokeTactics.Api.Test.Utils.Helpers;

public static class AbilityVerifier
{
    public static async Task VerifyAbilities(this PokeTacticsFixture fixture, IDictionary<string, AbilityEffectPokeApiResponse> abilityEffectsByNameMap)
    {
        IUnitOfWork unitOfWork = fixture.GetService<IUnitOfWork>();
        IEnumerable<Ability> allAbilities = await unitOfWork.AbilityDao.LoadAllAsync();
        
        foreach (var abilityEffectByName in abilityEffectsByNameMap)
        {
            Ability ability = Assert.Single(allAbilities, x => x.Name == abilityEffectByName.Key);
            Assert.Equal(abilityEffectByName.Value.EffectEntries.Single().Effect, ability.Description);
        }
    }

    public static void VerifyAbilityDtos(ICollection<AbilityInPokemon> expectedAbilitiesInPokemon, ICollection<AbilityDto> actualAbilities)
    {
        Assert.Equal(expectedAbilitiesInPokemon.Count, actualAbilities.Count);
        int numberOfAbilities = actualAbilities.Count;

        for (int i = 0; i < numberOfAbilities; i++)
        {
            VerifyAbilityDto(expectedAbilitiesInPokemon.ElementAt(i), actualAbilities.ElementAt(i));
        }
    }

    private static void VerifyAbilityDto(AbilityInPokemon expectedAbilityInPokemon, AbilityDto actualAbility)
    {
        Assert.Equal(expectedAbilityInPokemon.IsHidden, actualAbility.IsHidden);
        Assert.Equal(expectedAbilityInPokemon.Ability.Name, actualAbility.Name);
        Assert.Equal(expectedAbilityInPokemon.Ability.Description, actualAbility.Description);
    }
}
