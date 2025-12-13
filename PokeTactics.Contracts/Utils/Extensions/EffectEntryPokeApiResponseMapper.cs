using PokeTactics.Contracts.Constants;
using PokeTactics.Contracts.EffectEntry.PokeApi;

namespace PokeTactics.Contracts.Utils.Extensions;

public static class EffectEntryPokeApiResponseExtensions
{
    public static string? TakeEnglishToString(this ICollection<EffectEntryPokeApiResponse> effectEntryPokeApiResponses)
    {
        return effectEntryPokeApiResponses.SingleOrDefault(x =>
                x.Language.Name.Equals(ContractsConstants.LanguageOfEffectEntrySelected, StringComparison.OrdinalIgnoreCase))?.Effect;
    }
}
