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
            IQueryable<Pokemon> query = Query().OrderBy(x => x.Id);

            if (request.LastId.HasValue)
            {
                query = query.Where(x => x.Id > request.LastId.Value);
            }

            return await query
                .Take(request.PageSize)
                .ToListAsync();
        }

        public async Task<Pokemon?> LoadByName(string name)
        {
            return await Query()
                .SingleOrDefaultAsync(p => p.Name == name);
        }

        public async Task<IDictionary<string, Pokemon>> LoadMapByName()
        {
            return await Query()
                .ToDictionaryAsync(p => p.Name, p => p);
        }

        protected override IQueryable<Pokemon> Query()
        {
            return base.Query()
                .Include(p => p.MovesInPokemon)
                    .ThenInclude(mp => mp.Move)
                .Include(p => p.AbilitiesInPokemon)
                    .ThenInclude(ap => ap.Ability)
                .Include(p => p.Stats)
                .Include(p => p.Sprite);
        }
    }
}