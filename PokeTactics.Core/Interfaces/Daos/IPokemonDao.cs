using PokeTactics.Core.Definitions.Dtos;
using PokeTactics.Core.Entities;

namespace PokeTactics.Core.Interfaces.Daos
{
    public interface IPokemonDao : IBaseDao<Pokemon>
    {
        Task DeleteByNames(IEnumerable<string> names);

        Task<ICollection<Pokemon>> Find(KeysetPaginationRequest request);

        Task<ICollection<Pokemon>> FindDeep(KeysetPaginationRequest request);

        Task<Pokemon?> LoadByName(string name);

        Task<IDictionary<string, Pokemon>> LoadMapByName();
    }
}