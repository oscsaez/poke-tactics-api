using PokeTactics.Api.Test.Contexts;
using PokeTactics.Api.Test.Fixture;
using PokeTactics.Api.Test.Utils.Builders;
using PokeTactics.Contracts.Ability.PokeApi;
using PokeTactics.Contracts.Move.PokeApi;
using PokeTactics.Contracts.Pokemon.PokeApi;
using PokeTactics.Core.Interfaces.SyncServices;

namespace PokeTactics.Api.Test.Utils.Helpers;

public static class SetupTestHelper
{
    public static async Task SetupSyncPokemon(this PokeTacticsFixture fixture)
    {
        ISyncService syncService = fixture.GetService<ISyncService>();
        SyncTestContext context = fixture.SetupPokeApiWithNewPokemon();
        CancellationToken cancellationToken = new();

        await syncService.Sync(cancellationToken);

        await fixture.VerifyAbilitiesMovesAndPokemon(context);
    }

    public static SyncTestContext SetupPokeApiWithNewPokemon(this PokeTacticsFixture fixture)
    {
        string pokemonName = TestGenerator.RandomName();
        AbilitySummaryListPokeApiResponse abilitiesSummary = AbilityBuilder.BuildAbilitySummaryListPokeApiResponse();
        AbilitySummaryPokeApiResponse abilitySummary = abilitiesSummary.Results.Single();
        AbilityEffectPokeApiResponse abilityEffect = AbilityBuilder.BuildAbilityEffectPokeApiResponse();
        MoveSummaryListPokeApiResponse movesSummary = MoveBuilder.BuildMoveSummaryListPokeApiResponse();
        MoveSummaryPokeApiResponse moveSummary = movesSummary.Results.Single();
        MoveInfoPokeApiResponse moveInfo = MoveBuilder.BuildMoveInfoPokeApiResponse();
        PokemonSummaryListPokeApiResponse pokemonSummaryList = PokemonBuilder.BuildPokemonSummaryListPokeApiResponse(pokemonName);
        PokemonSummaryPokeApiResponse pokemonSummary = pokemonSummaryList.Results.Single();
        PokemonPokeApiResponse pokemonResponse = PokemonBuilder.BuildPokemonPokeApiResponse(pokemonName, [abilitySummary], [moveSummary]);

        fixture.ConfigurePokeApiMockServerToGetAbilitiesSummary(abilitiesSummary);
        fixture.ConfigurePokeApiMockServerForGet(abilitySummary.Url, abilityEffect);
        fixture.ConfigurePokeApiMockServerToGetMovesSummary(movesSummary);
        fixture.ConfigurePokeApiMockServerForGet(moveSummary.Url, moveInfo);
        fixture.ConfigurePokeApiMockServerToGetPokemonSummaryList(pokemonSummaryList);
        fixture.ConfigurePokeApiMockServerForGet(pokemonSummary.Url, pokemonResponse);

        return new SyncTestContext
        {
            AbilitySummaryResponse = abilitySummary,
            AbilityEffectResponse = abilityEffect,
            MoveSummaryResponse = moveSummary,
            MoveInfoResponse = moveInfo,
            PokemonSummaryResponse = pokemonSummary,
            PokemonResponse = pokemonResponse
        };
    }
}
