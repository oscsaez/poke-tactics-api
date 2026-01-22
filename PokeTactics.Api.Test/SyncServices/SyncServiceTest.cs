using PokeTactics.Api.Test.Contexts;
using PokeTactics.Api.Test.Fixture;
using PokeTactics.Api.Test.Utils;
using PokeTactics.Api.Test.Utils.Helpers;
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
        SyncTestContext context = _fixture.SetupPokeApiWithNewPokemon();

        // Act
        await _syncService.Sync(cancellationToken);

        // Assert
        await _fixture.VerifyAbilitiesMovesAndPokemon(context);
    }

    [Fact]
    public async Task Sync_UpdateAbilitiesMovesAndPokemon_UpdateThem()
    {
        // Arrange
        CancellationToken cancellationToken = new();
        SyncTestContext context = _fixture.SetupPokeApiWithNewPokemon();

        await _syncService.Sync(cancellationToken);

        context.AbilityEffectResponse!.EffectEntries.Single().Effect = TestGenerator.RandomGuidAsString();
        _fixture.ConfigurePokeApiMockServerForGet(context.AbilitySummaryResponse!.Url, context.AbilityEffectResponse!);

        context.MoveInfoResponse!.EffectEntries.Single().Effect = TestGenerator.RandomGuidAsString();
        _fixture.ConfigurePokeApiMockServerForGet(context.MoveSummaryResponse!.Url, context.MoveInfoResponse!);

        context.PokemonResponse!.Order = TestGenerator.RandomInt();
        _fixture.ConfigurePokeApiMockServerForGet(context.PokemonSummaryResponse!.Url, context.PokemonResponse!);

        // Act
        await _syncService.Sync(cancellationToken);

        // Assert
        await _fixture.VerifyAbilitiesMovesAndPokemon(context);
    }

    [Fact]
    public async Task Sync_RemoveAbilitiesMovesAndPokemonAndAddNewOnes_RemoveExistingOnesAndAddNewOnes()
    {
        // Arrange
        CancellationToken cancellationToken = new();
        SyncTestContext context1 = _fixture.SetupPokeApiWithNewPokemon();

        await _syncService.Sync(cancellationToken);
        await _fixture.VerifyAbilitiesMovesAndPokemon(context1);

        SyncTestContext context2 = _fixture.SetupPokeApiWithNewPokemon();

        // Act
        await _syncService.Sync(cancellationToken);

        // Arrange
        await _fixture.VerifyAbilitiesMovesAndPokemon(context2);
    }

}
