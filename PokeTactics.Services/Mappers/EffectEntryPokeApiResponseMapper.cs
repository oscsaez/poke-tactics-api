using PokeTactics.Contracts.EffectEntry.PokeApi;
using PokeTactics.Services.Constants;

namespace PokeTactics.Services.Mappers;

public static class EffectEntryPokeApiResponseMapper
{
    public static string? TakeEnglishToString(this ICollection<EffectEntryPokeApiResponse> effectEntryPokeApiResponses)
    {
        return effectEntryPokeApiResponses.SingleOrDefault(x =>
                x.Language.Name.Equals(ServiceConstants.LanguageOfEffectEntrySelected, StringComparison.OrdinalIgnoreCase))?.Effect;
    }
}
