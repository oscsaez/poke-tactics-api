using System;

namespace PokeTactics.Core.Interfaces.SyncServices;

public interface ISyncService
{
    Task Sync(CancellationToken cancellationToken);
}
