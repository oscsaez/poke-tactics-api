using PokeTactics.Core.Definitions.Dtos;
using PokeTactics.Core.Entities;

namespace PokeTactics.Services.Facade;

public interface IPokemonService
{
    Task<KeysetPaginationResponse<Pokemon>> Find(KeysetPaginationRequest request);
}
