using PokeTactics.Contracts.Ability.PokeApi;

namespace PokeTactics.Api.Test.Utils.Builders;

public static class AbilityBuilder
{
    public static AbilitySummaryListPokeApiResponse BuildAbilitySummaryListPokeApiResponse()
    {
        return new AbilitySummaryListPokeApiResponse
        {
            Count = 1,
            Results = [
                new AbilitySummaryPokeApiResponse
                {
                    Name = TestGenerator.RandomName(),
                    Url = TestGenerator.RandomPath()
                }
            ]
        };
    }

    public static AbilityEffectPokeApiResponse BuildAbilityEffectPokeApiResponse()
    {
        return new AbilityEffectPokeApiResponse
        {
            EffectEntries = [EffectEntryBuilder.BuildRandomEffectEntry()]
        };
    }
}
