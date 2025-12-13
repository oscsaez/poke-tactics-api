using Microsoft.EntityFrameworkCore;
using PokeTactics.Core.Entities;
using PokeTactics.Core.Interfaces.Daos;
using PokeTactics.Infrastructure.Data;

namespace PokeTactics.Infrastructure.Daos;

public class AbilityDao : BaseDao<Ability>, IAbilityDao
{
    public AbilityDao(PokeTacticsContext dbContext) : base(dbContext)
    {
    }

    public async Task DeleteByNames(IEnumerable<string> names)
    {
        await Query()
            .Where(x => names.Contains(x.Name))
            .ExecuteDeleteAsync();
    }

    public async Task<ICollection<Ability>> LoadByNames(IEnumerable<string> names)
    {
        return await Query()
            .Where(a => names.Contains(a.Name))
            .ToListAsync();
    }

    public async Task<IDictionary<string, Ability>> LoadMapByName()
    {
        return await Query()
            .ToDictionaryAsync(a => a.Name, a => a);
    }
}
