using PokeTactics.Core.Entities;

namespace PokeTactics.Core.Interfaces.Daos
{
    public interface IPokemonDao : IBaseDao<Pokemon>
    {
        Task DeleteByNames(IEnumerable<string> names);

        Task<Pokemon?> LoadByName(string name);

        Task<IDictionary<string, Pokemon>> LoadMapByName();
    }
}