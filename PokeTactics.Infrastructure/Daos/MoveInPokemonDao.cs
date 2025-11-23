using PokeTactics.Core.Entities;
using PokeTactics.Core.Interfaces.Daos;
using PokeTactics.Infrastructure.Data;

namespace PokeTactics.Infrastructure.Daos;

public class MoveInPokemonDao : BaseDao<MoveInPokemon>, IMoveInPokemonDao
{
    public MoveInPokemonDao(PokeTacticsContext dbContext) : base(dbContext)
    {
    }
}
