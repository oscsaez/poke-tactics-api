using PokeTactics.Contracts.Common.Requests;
using PokeTactics.Contracts.Common.Responses;
using PokeTactics.Contracts.Pokemon.Responses;

namespace PokeTactics.Services.Facade;

public interface IPokemonService
{
    Task<KeysetPaginationResponse<PokemonDto>> Find(KeysetPaginationRequest request);

    Task<KeysetPaginationResponse<PokemonDto>> FindDeep(KeysetPaginationRequest request);
}
