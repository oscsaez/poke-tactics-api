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

        public async Task<int> CountAsync()
        {
            return await DbSet.CountAsync();
        }

        public async Task<Pokemon?> LoadByName(string name)
        {
            return await DbSet
                .Include(p => p.MovesInPokemon)
                    .ThenInclude(mp => mp.Move)
                .Include(p => p.AbilitiesInPokemon)
                    .ThenInclude(ap => ap.Ability)
                .Include(p => p.Stats)
                .Include(p => p.Sprite)
                .SingleOrDefaultAsync(p => p.Name == name);
        }
    }
}