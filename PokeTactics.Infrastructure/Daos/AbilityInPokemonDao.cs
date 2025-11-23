using PokeTactics.Core.Entities;
using PokeTactics.Core.Interfaces.Daos;
using PokeTactics.Infrastructure.Data;

namespace PokeTactics.Infrastructure.Daos;

public class AbilityInPokemonDao : BaseDao<AbilityInPokemon>, IAbilityInPokemonDao
{
    public AbilityInPokemonDao(PokeTacticsContext dbContext) : base(dbContext)
    {
    }
}
