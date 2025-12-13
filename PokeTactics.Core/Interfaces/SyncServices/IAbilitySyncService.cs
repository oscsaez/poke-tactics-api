namespace PokeTactics.Core.Interfaces.SyncServices;

public interface IAbilitySyncService
{
    Task Sync(CancellationToken cancellationToken);
}
