using PokeTactics.Contracts.Language.PokeApi;

namespace PokeTactics.Contracts.EffectEntry.PokeApi;

public class EffectEntryPokeApiResponse
{
    public required string Effect { get; set; }

    public required LanguagePokeApiResponse Language { get; set; }
}
