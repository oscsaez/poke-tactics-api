using PokeTactics.Core.Definitions.Dtos;
using PokeTactics.Core.Entities;
using PokeTactics.Core.Interfaces;

namespace PokeTactics.Services.Facade.Impl;

public class PokemonService : IPokemonService
{
    private readonly IUnitOfWork _unitOfWork;

    public PokemonService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<KeysetPaginationResponse<Pokemon>> Find(KeysetPaginationRequest request)
    {
        ICollection<Pokemon> pokemonList = await _unitOfWork.PokemonDao.Find(request);

        return new KeysetPaginationResponse<Pokemon>(
            Items: pokemonList,
            NextLastId: pokemonList.LastOrDefault()?.Id
        );
    }
}
