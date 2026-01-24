using Microsoft.EntityFrameworkCore;
using PokeTactics.Core.Entities;
using PokeTactics.Core.Interfaces.Daos;
using PokeTactics.Infrastructure.Data;

namespace PokeTactics.Infrastructure.Daos
{
    public class PokemonDao : BaseDao<Pokemon>, IPokemonDao
    {
        public PokemonDao(PokeTacticsContext dbContext) : base(dbContext)
        {
        }

        public async Task DeleteByNames(IEnumerable<string> names)
        {
            await Query()
                .Where(x => names.Contains(x.Name))
                .ExecuteDeleteAsync();
        }

        public async Task<ICollection<Pokemon>> Find(int pageSize, int? lastId)
        {
            IQueryable<Pokemon> query = Query()
                .Include(x => x.Sprite)
                .Include(x => x.Stats);

            return await Find(pageSize, lastId, query);
        }

        public async Task<ICollection<Pokemon>> FindDeep(int pageSize, int? lastId)
        {
            return await Find(pageSize, lastId, DeepQuery());
        }

        public async Task<Pokemon?> LoadByName(string name)
        {
            return await Query()
                .SingleOrDefaultAsync(p => p.Name == name);
        }

        public async Task<IDictionary<string, Pokemon>> LoadMapByName()
        {
            return await DeepQuery()
                .ToDictionaryAsync(p => p.Name, p => p);
        }

        private static async Task<ICollection<Pokemon>> Find(int pageSize, int? lastId, IQueryable<Pokemon> query)
        {
            IQueryable<Pokemon> filterQuery = query
                .OrderBy(x => x.PokedexOrder == null || x.PokedexOrder < 0)
                .ThenBy(x => x.PokedexOrder)
                .ThenBy(x => x.Id);

            if (lastId.HasValue)
            {
                filterQuery = query.Where(x => x.Id > lastId.Value);
            }

            return await filterQuery
                .Take(pageSize)
                .ToListAsync();
        }

        private IQueryable<Pokemon> DeepQuery()
        {
            return Query()
                .Include(p => p.MovesInPokemon)
                    .ThenInclude(mp => mp.Move)
                .Include(p => p.AbilitiesInPokemon)
                    .ThenInclude(ap => ap.Ability)
                .Include(p => p.Stats)
                .Include(p => p.Sprite);
        }
    }
}