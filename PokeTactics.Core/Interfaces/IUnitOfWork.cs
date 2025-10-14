using PokeTactics.Core.Interfaces.Daos;

namespace PokeTactics.Core.Interfaces
{
    // Unit of work pattern
    public interface IUnitOfWork : IDisposable
    {
        // Set here DAOs to use their methods as single transactions
        IPokemonDao PokemonDao { get; }

        IAbilityDao AbilityDao { get; }

        IMoveDao MoveDao { get; }

        Task<int> CommitAsync();
    }
}