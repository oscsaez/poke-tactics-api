using PokeTactics.Contracts.Ability.PokeApi;
using PokeTactics.Contracts.Move.PokeApi;
using PokeTactics.Contracts.Pokemon.PokeApi;

namespace PokeTactics.Api.Test.Contexts;

public class SyncTestContext
{
    public ICollection<AbilitySummaryPokeApiResponse> AbilitySummaryResponses { get; set; } = [];

    public IDictionary<string, AbilityEffectPokeApiResponse> AbilityEffectResponsesByName { get; set; } =
        new Dictionary<string, AbilityEffectPokeApiResponse>();

    public ICollection<MoveSummaryPokeApiResponse> MoveSummaryResponses { get; set; } = [];

    public IDictionary<string, MoveInfoPokeApiResponse> MoveInfoResponsesByName { get; set; } = new Dictionary<string, MoveInfoPokeApiResponse>();

    public ICollection<PokemonSummaryPokeApiResponse> PokemonSummaryResponses { get; set; } = [];

    public ICollection<PokemonPokeApiResponse> PokemonResponses { get; set; } = [];
}
