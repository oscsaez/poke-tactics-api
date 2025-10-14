using PokeTactics.Core.Entities;

namespace PokeTactics.Core.Interfaces.Daos
{
    public interface IPokemonDao : IBaseDao<Pokemon>
    {
        Task<int> CountAsync();

        Task<Pokemon?> LoadByName(string name);
    }
}