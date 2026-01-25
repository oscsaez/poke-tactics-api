using PokeTactics.Contracts.Ability.PokeApi;

namespace PokeTactics.Api.Test.Utils.Builders;

public static class AbilityBuilder
{
    public static AbilitySummaryListPokeApiResponse BuildAbilitySummaryListPokeApiResponse()
    {
        const int One = 1;

        return BuildAbilitySummaryListPokeApiResponse(One);
    }

    public static AbilitySummaryListPokeApiResponse BuildAbilitySummaryListPokeApiResponse(int numberOfAbilities)
    {
        List<AbilitySummaryPokeApiResponse> abilitySummaries = new(numberOfAbilities);

        for (int i = 0; i < numberOfAbilities; i++)
        {
            abilitySummaries.Add(new AbilitySummaryPokeApiResponse
            {
                Name = TestGenerator.RandomName(),
                Url = TestGenerator.RandomPath()
            });
        }

        return new AbilitySummaryListPokeApiResponse
        {
            Count = numberOfAbilities,
            Results = abilitySummaries
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
