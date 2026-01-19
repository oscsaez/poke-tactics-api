using Microsoft.EntityFrameworkCore;
using PokeTactics.Core.Definitions.Dtos;
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

        public async Task<ICollection<Pokemon>> Find(KeysetPaginationRequest request)
        {
            IQueryable<Pokemon> query = Query().Include(x => x.Sprite);

            return await Find(request, query);
        }

        public async Task<ICollection<Pokemon>> FindDeep(KeysetPaginationRequest request)
        {
            return await Find(request, DeepQuery());
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

        private async Task<ICollection<Pokemon>> Find(KeysetPaginationRequest request, IQueryable<Pokemon> query)
        {
            IQueryable<Pokemon> filterQuery = query
                .OrderBy(x => x.PokedexOrder == null || x.PokedexOrder < 0)
                .ThenBy(x => x.PokedexOrder)
                .ThenBy(x => x.Id);

            if (request.LastId.HasValue)
            {
                filterQuery = query.Where(x => x.Id > request.LastId.Value);
            }

            return await filterQuery
                .Take(request.PageSize)
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