using System;
using PokeTactics.Core.Interfaces.SyncServices;

namespace PokeTactics.Api.SyncServices;

public class SyncService : ISyncService
{
    private readonly IAbilitySyncService _abilitySyncService;
    private readonly IMoveSyncService _moveSyncService;
    private readonly IPokemonSyncService _pokemonSyncService;

    public SyncService(
        IAbilitySyncService abilitySyncService, 
        IMoveSyncService moveSyncService, 
        IPokemonSyncService pokemonSyncService)
    {
        _abilitySyncService = abilitySyncService;
        _moveSyncService = moveSyncService;
        _pokemonSyncService = pokemonSyncService;
    }

    public async Task Sync(CancellationToken cancellationToken)
    {
        await _abilitySyncService.Sync(cancellationToken);
        await _moveSyncService.Sync(cancellationToken);
        await _pokemonSyncService.Sync(cancellationToken);
    }
}
