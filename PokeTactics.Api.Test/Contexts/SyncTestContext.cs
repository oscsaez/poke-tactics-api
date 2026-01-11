using PokeTactics.Contracts.Ability.PokeApi;
using PokeTactics.Contracts.Move.PokeApi;
using PokeTactics.Contracts.Pokemon.PokeApi;

namespace PokeTactics.Api.Test.Contexts;

public class SyncTestContext
{
    public AbilitySummaryPokeApiResponse? AbilitySummaryResponse { get; set; }

    public AbilityEffectPokeApiResponse? AbilityEffectResponse { get; set; }

    public MoveSummaryPokeApiResponse? MoveSummaryResponse { get; set; }

    public MoveInfoPokeApiResponse? MoveInfoResponse { get; set; }

    public PokemonSummaryPokeApiResponse? PokemonSummaryResponse { get; set; }

    public PokemonPokeApiResponse? PokemonResponse { get; set; }
}
