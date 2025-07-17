namespace PokeTactics.Contracts.Move.PokeApi;

public class EffectEntryPokeApiResponse
{
    public required string Effect { get; set; }

    public required LanguagePokeApiResponse Language { get; set; }
}
