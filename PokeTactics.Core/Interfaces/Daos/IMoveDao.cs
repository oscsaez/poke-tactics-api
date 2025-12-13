using PokeTactics.Core.Entities;

namespace PokeTactics.Core.Interfaces.Daos;

public interface IMoveDao : IBaseDao<Move>
{
    Task DeleteByNames(IEnumerable<string> names);

    Task<ICollection<Move>> LoadByNames(IEnumerable<string> names);

    Task<IDictionary<string, Move>> LoadMapByName();
}
