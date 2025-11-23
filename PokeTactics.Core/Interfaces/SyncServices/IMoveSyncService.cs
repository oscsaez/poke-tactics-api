namespace PokeTactics.Core.Interfaces.SyncServices;

public interface IMoveSyncService
{
    Task Sync(CancellationToken cancellationToken);
}
