namespace PokeTactics.Core.Interfaces
{
    // Unit of work pattern
    public interface IUnitOfWork : IDisposable
    {
        // Set here DAOs to use their methods as single transactions

        Task<int> CommitAsync();
    }
}