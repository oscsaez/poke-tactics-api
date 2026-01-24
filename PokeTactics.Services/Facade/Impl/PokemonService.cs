using PokeTactics.Contracts.Common.Requests;
using PokeTactics.Contracts.Common.Responses;
using PokeTactics.Contracts.Pokemon.Responses;
using PokeTactics.Core.Entities;
using PokeTactics.Core.Exceptions;
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
        return await ExecuteFind(request, () => _unitOfWork.PokemonDao.Find(request.PageSize, request.LastPokedexOrder, request.LastId));
    }

    public async Task<KeysetPaginationResponse<PokemonDto>> FindDeep(KeysetPaginationRequest request)
    {
        return await ExecuteFind(request, () => _unitOfWork.PokemonDao.FindDeep(request.PageSize, request.LastPokedexOrder, request.LastId));
    }

    private static async Task<KeysetPaginationResponse<PokemonDto>> ExecuteFind(KeysetPaginationRequest request, Func<Task<ICollection<Pokemon>>> find)
    {
        if (request.HasInvalidCursor)
        {
            throw new InvalidRequestException($"Both {nameof(KeysetPaginationRequest.LastPokedexOrder)} and {nameof(KeysetPaginationRequest.LastId)} " +
                "or no one must be provided");
        }

        ICollection<Pokemon> pokemonList = await find();

        Pokemon? lastPokemon = pokemonList.LastOrDefault();

        return new KeysetPaginationResponse<PokemonDto>(
            Items: pokemonList.ToPokemonDtos(),
            NextLastPokedexOrder: lastPokemon?.PokedexOrder,
            NextLastId: lastPokemon?.Id
        );
    }
}
