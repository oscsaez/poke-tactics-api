namespace PokeTactics.Core.Interfaces.SyncServices;

public interface IPokemonSyncService
{
    Task Sync(CancellationToken cancellationToken);
}
