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
    public static async Task SetupSyncSinglePokemon(this PokeTacticsFixture fixture)
    {
        ISyncService syncService = fixture.GetService<ISyncService>();
        SyncTestContext context = fixture.SetupPokeApiWithSingleNewPokemon();
        CancellationToken cancellationToken = new();

        await syncService.Sync(cancellationToken);

        await fixture.VerifyAbilities(context.AbilityEffectResponsesByName);
        await fixture.VerifyMoves(context.MoveInfoResponsesByName);
        await fixture.VerifyPokemon(context.PokemonResponses);
    }

    public static async Task SetupSyncPokemon(this PokeTacticsFixture fixture, int numberOfPokemon)
    {
        ISyncService syncService = fixture.GetService<ISyncService>();
        SyncTestContext context = fixture.SetupPokeApiWithNewPokemon(numberOfPokemon);
        CancellationToken cancellationToken = new();

        await syncService.Sync(cancellationToken);

        await fixture.VerifyAbilities(context.AbilityEffectResponsesByName);
        await fixture.VerifyMoves(context.MoveInfoResponsesByName);
        await fixture.VerifyPokemon(context.PokemonResponses);
    }

    public static SyncTestContext SetupPokeApiWithSingleNewPokemon(this PokeTacticsFixture fixture)
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
            AbilityEffectResponsesByName = new Dictionary<string, AbilityEffectPokeApiResponse> { { abilitySummary.Name, abilityEffect } },
            AbilitySummaryResponses = abilitiesSummary.Results,
            MoveInfoResponsesByName = new Dictionary<string, MoveInfoPokeApiResponse> { { moveSummary.Name, moveInfo } },
            MoveSummaryResponses = movesSummary.Results,
            PokemonResponses = [pokemonResponse],
            PokemonSummaryResponses = pokemonSummaryList.Results
        };
    }

    private static SyncTestContext SetupPokeApiWithNewPokemon(this PokeTacticsFixture fixture, int numberOfPokemon)
    {
        List<string> pokemonNames = new(numberOfPokemon);

        for (int i = 0; i < numberOfPokemon; i++)
        {
            pokemonNames.Add(TestGenerator.RandomName());
        }

        AbilitySummaryListPokeApiResponse abilitiesSummary = AbilityBuilder.BuildAbilitySummaryListPokeApiResponse(numberOfPokemon);
        MoveSummaryListPokeApiResponse movesSummary = MoveBuilder.BuildMoveSummaryListPokeApiResponse(numberOfPokemon);
        PokemonSummaryListPokeApiResponse pokemonSummaryList = PokemonBuilder.BuildPokemonSummaryListPokeApiResponse(pokemonNames);

        fixture.ConfigurePokeApiMockServerToGetAbilitiesSummary(abilitiesSummary);
        fixture.ConfigurePokeApiMockServerToGetMovesSummary(movesSummary);
        fixture.ConfigurePokeApiMockServerToGetPokemonSummaryList(pokemonSummaryList);
        
        Dictionary<string, AbilityEffectPokeApiResponse> abilityEffectsByName = [];
        Dictionary<string, MoveInfoPokeApiResponse> movesInfoByName = [];
        List<PokemonPokeApiResponse> pokemonResponses = [];
        for (int i = 0; i < numberOfPokemon; i++)
        {
            AbilitySummaryPokeApiResponse abilitySummary = abilitiesSummary.Results.ElementAt(i);
            MoveSummaryPokeApiResponse moveSummary = movesSummary.Results.ElementAt(i);
            PokemonSummaryPokeApiResponse pokemonSummary = pokemonSummaryList.Results.ElementAt(i);

            PokemonPokeApiResponse pokemonResponse = PokemonBuilder.BuildPokemonPokeApiResponse(
                pokemonNames[i], 
                [abilitySummary],
                movesSummary.Results);

            AbilityEffectPokeApiResponse abilityEffect = AbilityBuilder.BuildAbilityEffectPokeApiResponse();
            MoveInfoPokeApiResponse moveInfo = MoveBuilder.BuildMoveInfoPokeApiResponse();

            fixture.ConfigurePokeApiMockServerForGet(abilitySummary.Url, abilityEffect);
            fixture.ConfigurePokeApiMockServerForGet(moveSummary.Url, moveInfo);
            fixture.ConfigurePokeApiMockServerForGet(pokemonSummary.Url, pokemonResponse);

            abilityEffectsByName.Add(abilitySummary.Name, abilityEffect);
            movesInfoByName.Add(moveSummary.Name, moveInfo);
            pokemonResponses.Add(pokemonResponse);
        }

        return new SyncTestContext
        {
            AbilityEffectResponsesByName = abilityEffectsByName,
            AbilitySummaryResponses = abilitiesSummary.Results,
            MoveInfoResponsesByName = movesInfoByName,
            MoveSummaryResponses = movesSummary.Results,
            PokemonResponses = pokemonResponses,
            PokemonSummaryResponses = pokemonSummaryList.Results
        };
    }
}
