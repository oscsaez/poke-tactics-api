using PokeTactics.Core.Entities;

namespace PokeTactics.Core.Interfaces.Daos;

public interface IAbilityDao : IBaseDao<Ability>
{
    Task<ICollection<Ability>> LoadByNames(IEnumerable<string> names);

    Task<IDictionary<string, Ability>> LoadMapByName();
}
