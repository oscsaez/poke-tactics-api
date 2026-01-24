using PokeTactics.Api.Test.Contexts;
using PokeTactics.Api.Test.Fixture;
using PokeTactics.Api.Test.Utils;
using PokeTactics.Api.Test.Utils.Helpers;
using PokeTactics.Contracts.Ability.PokeApi;
using PokeTactics.Contracts.Move.PokeApi;
using PokeTactics.Contracts.Pokemon.PokeApi;
using PokeTactics.Core.Interfaces;
using PokeTactics.Core.Interfaces.SyncServices;

namespace PokeTactics.Api.Test.SyncServices;

[Collection(PokeTacticsCollection.Name)]
public class SyncServiceTest : IAsyncLifetime
{
    private readonly PokeTacticsFixture _fixture;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISyncService _syncService;

    public SyncServiceTest(PokeTacticsFixture fixture)
    {
        _fixture = fixture;
        _unitOfWork = _fixture.GetService<IUnitOfWork>();
        _syncService = _fixture.GetService<ISyncService>();
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _fixture.DeleteAll();
    }

    [Fact]
    public async Task Sync_NewAbilitiesMovesAndPokemon_CreateThem()
    {
        // Arrange
        CancellationToken cancellationToken = new();
        SyncTestContext context = _fixture.SetupPokeApiWithSingleNewPokemon();

        // Act
        await _syncService.Sync(cancellationToken);

        // Assert
        await _fixture.VerifyAbilities(context.AbilityEffectResponsesByName);
        await _fixture.VerifyMoves(context.MoveInfoResponsesByName);
        await _fixture.VerifyPokemon(context.PokemonResponses);
    }

    [Fact]
    public async Task Sync_UpdateAbilitiesMovesAndPokemon_UpdateThem()
    {
        // Arrange
        CancellationToken cancellationToken = new();
        SyncTestContext context = _fixture.SetupPokeApiWithSingleNewPokemon();

        await _syncService.Sync(cancellationToken);

        AbilityEffectPokeApiResponse updatedAbilityEffect = context.AbilityEffectResponsesByName!.Values.First();
        string abilitySummaryUri = context.AbilitySummaryResponses.First().Url;

        updatedAbilityEffect.EffectEntries.First().Effect = TestGenerator.RandomGuidAsString();
        _fixture.ConfigurePokeApiMockServerForGet(abilitySummaryUri, updatedAbilityEffect);

        MoveInfoPokeApiResponse updatedMoveInfo = context.MoveInfoResponsesByName.Values.First();
        string moveSummaryUri = context.MoveSummaryResponses.First().Url;

        updatedMoveInfo.EffectEntries.Single().Effect = TestGenerator.RandomGuidAsString();
        _fixture.ConfigurePokeApiMockServerForGet(moveSummaryUri, updatedMoveInfo);

        PokemonPokeApiResponse updatedPokemon = context.PokemonResponses.First();
        string pokemonSummaryUri = context.PokemonSummaryResponses.First().Url;

        updatedPokemon.Order = TestGenerator.RandomInt();
        _fixture.ConfigurePokeApiMockServerForGet(pokemonSummaryUri, updatedPokemon);

        // Act
        await _syncService.Sync(cancellationToken);

        // Assert
        await _fixture.VerifyAbilities(context.AbilityEffectResponsesByName);
        await _fixture.VerifyMoves(context.MoveInfoResponsesByName);
        await _fixture.VerifyPokemon(context.PokemonResponses);
    }

    [Fact]
    public async Task Sync_RemoveAbilitiesMovesAndPokemonAndAddNewOnes_RemoveExistingOnesAndAddNewOnes()
    {
        // Arrange
        CancellationToken cancellationToken = new();
        SyncTestContext context1 = _fixture.SetupPokeApiWithSingleNewPokemon();

        await _syncService.Sync(cancellationToken);
        await _fixture.VerifyAbilities(context1.AbilityEffectResponsesByName);
        await _fixture.VerifyMoves(context1.MoveInfoResponsesByName);
        await _fixture.VerifyPokemon(context1.PokemonResponses);

        SyncTestContext context2 = _fixture.SetupPokeApiWithSingleNewPokemon();

        // Act
        await _syncService.Sync(cancellationToken);

        // Arrange
        await _fixture.VerifyAbilities(context2.AbilityEffectResponsesByName);
        await _fixture.VerifyMoves(context2.MoveInfoResponsesByName);
        await _fixture.VerifyPokemon(context2.PokemonResponses);
    }

}
