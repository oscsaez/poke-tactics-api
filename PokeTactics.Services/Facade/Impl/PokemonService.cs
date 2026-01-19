using PokeTactics.Contracts.Pokemon.Responses;
using PokeTactics.Core.Definitions.Dtos;
using PokeTactics.Core.Entities;
using PokeTactics.Core.Interfaces;
using PokeTactics.Services.Mappers;

namespace PokeTactics.Services.Facade.Impl;

public class PokemonService : IPokemonService
{
    private readonly IUnitOfWork _unitOfWork;

    public PokemonService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<KeysetPaginationResponse<PokemonDto>> Find(KeysetPaginationRequest request)
    {
        return await ExecuteFind(() => _unitOfWork.PokemonDao.Find(request));
    }

    public async Task<KeysetPaginationResponse<PokemonDto>> FindDeep(KeysetPaginationRequest request)
    {
        return await ExecuteFind(() => _unitOfWork.PokemonDao.FindDeep(request));
    }

    private static async Task<KeysetPaginationResponse<PokemonDto>> ExecuteFind(Func<Task<ICollection<Pokemon>>> find)
    {
        ICollection<Pokemon> pokemonList = await find();

        return new KeysetPaginationResponse<PokemonDto>(
            Items: pokemonList.ToPokemonDtos(),
            NextLastId: pokemonList.LastOrDefault()?.Id
        );
    }
}
