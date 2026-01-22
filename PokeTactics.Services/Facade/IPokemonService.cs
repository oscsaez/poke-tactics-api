using PokeTactics.Contracts.Pokemon.Responses;
using PokeTactics.Core.Definitions.Dtos;

namespace PokeTactics.Services.Facade;

public interface IPokemonService
{
    Task<KeysetPaginationResponse<PokemonDto>> Find(KeysetPaginationRequest request);

    Task<KeysetPaginationResponse<PokemonDto>> FindDeep(KeysetPaginationRequest request);
}
