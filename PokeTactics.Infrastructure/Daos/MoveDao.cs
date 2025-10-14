using Microsoft.EntityFrameworkCore;
using PokeTactics.Core.Entities;
using PokeTactics.Core.Interfaces.Daos;
using PokeTactics.Infrastructure.Data;

namespace PokeTactics.Infrastructure.Daos;

public class MoveDao : BaseDao<Move>, IMoveDao
{
    public MoveDao(PokeTacticsContext dbContext) : base(dbContext)
    {
    }

    public async Task<ICollection<Move>> LoadByNames(IEnumerable<string> names)
    {
        return await DbSet
            .Where(m => names.Contains(m.Name))
            .ToListAsync();
    }

    public async Task<IDictionary<string, Move>> LoadMapByName()
    {
        return await DbSet
            .ToDictionaryAsync(m => m.Name, m => m);
    }
}
