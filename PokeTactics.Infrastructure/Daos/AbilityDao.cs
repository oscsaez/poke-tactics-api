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

    public async Task<ICollection<Ability>> LoadByNames(IEnumerable<string> names)
    {
        return await DbSet
            .Where(a => names.Contains(a.Name))
            .ToListAsync();
    }
}
