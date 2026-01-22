using PokeTactics.Contracts.EffectEntry.PokeApi;
using PokeTactics.Contracts.Language.PokeApi;

namespace PokeTactics.Api.Test.Utils.Builders;

public static class EffectEntryBuilder
{
    private const string EnglishLanguage = "en";

    public static EffectEntryPokeApiResponse BuildRandomEffectEntry()
    {
        return new EffectEntryPokeApiResponse
        {
            Effect = TestGenerator.RandomGuidAsString(),
            Language = new LanguagePokeApiResponse
            {
                Name = EnglishLanguage
            }
        };
    }
}
