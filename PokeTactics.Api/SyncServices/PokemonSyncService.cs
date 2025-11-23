using System.Collections.Concurrent;
using PokeTactics.Api.Utils;
using PokeTactics.Contracts.Ability.PokeApi;
using PokeTactics.Contracts.Pokemon.PokeApi;
using PokeTactics.Core.Entities;
using PokeTactics.Core.Exceptions;
using PokeTactics.Core.Interfaces;
using PokeTactics.Core.Interfaces.SyncServices;
using PokeTactics.Core.Utils.Extensions;
using PokeTactics.Services.Mappers;

namespace PokeTactics.Api.SyncServices;

public class PokemonSyncService : IPokemonSyncService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AbilitySyncService> _logger;
    private readonly HttpClient _httpClient;
    private readonly SemaphoreSlim _semaphore;

    public PokemonSyncService(IUnitOfWork unitOfWork, ILogger<AbilitySyncService> logger, IHttpClientFactory httpClientFactory)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient(ApiConstants.ExternalApiName);
        _semaphore = new SemaphoreSlim(ApiConstants.NumberOfRequestsGrantedConcurrently);
    }

    public async Task Sync(CancellationToken cancellationToken)
    {
        IDictionary<string, Pokemon> existingPokemonMap = await _unitOfWork.PokemonDao.LoadMapByName();
        IDictionary<string, Ability> existingAbilitiesMap = await _unitOfWork.AbilityDao.LoadMapByName();
        IDictionary<string, Move> existingMovesMap = await _unitOfWork.MoveDao.LoadMapByName();

        PokemonSummaryListPokeApiResponse allPokemonResponse =
            await _httpClient.GetFromJsonAsync<PokemonSummaryListPokeApiResponse>(ApiConstants.AllPokemonInfoUri, cancellationToken)
            ?? throw new PokeApiSyncException(MessagesHelper.GetApiCallFailMessage(ApiConstants.AllPokemonInfoUri));

        ConcurrentBag<Pokemon> newPokemonConcurrentList = [];
        ConcurrentBag<Pokemon> updatedPokemonConcurrentList = [];
        ConcurrentBag<PokemonPokeApiResponse> newPokemonConcurrentResponses = [];
        List<Task> apiCalls = [];

        foreach (PokemonSummaryPokeApiResponse pokemonSummary in allPokemonResponse.Results)
        {
            await _semaphore.WaitAsync(cancellationToken);

            apiCalls.Add(Task.Run(async () =>
            {
                try
                {
                    PokemonPokeApiResponse pokemonInfoResponse =
                        await _httpClient.GetFromJsonAsync<PokemonPokeApiResponse>(pokemonSummary.Url, cancellationToken)
                        ?? throw new PokeApiSyncException(MessagesHelper.GetApiCallFailMessage(pokemonSummary.Url));

                    if (existingPokemonMap.TryGetValue(pokemonSummary.Name, out Pokemon existingPokemon))
                    {
                        Pokemon pokemonFromResponse = pokemonInfoResponse.ToPokemonWithAbilitiesAndMoves();

                        if (existingPokemon.Compare(pokemonFromResponse))
                        {
                            return;
                        }

                        existingPokemon.MapExisting(pokemonFromResponse, existingAbilitiesMap, existingMovesMap);
                        updatedPokemonConcurrentList.Add(existingPokemon);
                    }
                    else
                    {
                        Pokemon pokemonFromResponse = pokemonInfoResponse.ToPokemon();

                        newPokemonConcurrentList.Add(pokemonFromResponse);
                        newPokemonConcurrentResponses.Add(pokemonInfoResponse);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while trying to get new pokemon or the ones to be updated.");
                }
                finally
                {
                    _semaphore.Release();
                }
            }, cancellationToken));
        }

        await Task.WhenAll(apiCalls);

        await _unitOfWork.PokemonDao.CreateRangeAsync(newPokemonConcurrentList.ToList());
        await _unitOfWork.PokemonDao.UpdateRangeAsync(updatedPokemonConcurrentList.ToList());
        await _unitOfWork.CommitAsync();

        if(!newPokemonConcurrentList.IsNullOrEmpty())
        {
            List<PokemonPokeApiResponse> newPokemonResponses = newPokemonConcurrentResponses.ToList();

            await CreateAbilitiesInPokemon(newPokemonResponses);
            await CreateMovesInPokemon(newPokemonResponses);
        }

        _logger.LogInformation("Creation/Update of pokemon has been done!");
    }

    private async Task CreateAbilitiesInPokemon(ICollection<PokemonPokeApiResponse> pokemonResponses)
    {
        IEnumerable<KeyValuePair<string, AbilitySlotPokeApiResponse>> pokemonAndAbilityNamesPairs = 
            from p in pokemonResponses
            from a in p.Abilities
            select new KeyValuePair<string, AbilitySlotPokeApiResponse>(p.Name, a);

        IDictionary<string, Pokemon> existingPokemonMap = await _unitOfWork.PokemonDao.LoadMapByName();
        IDictionary<string, Ability> existingAbilitiesMap = await  _unitOfWork.AbilityDao.LoadMapByName();
        
        List<AbilityInPokemon> newAbilitiesInPokemon = [];
        foreach (KeyValuePair<string, AbilitySlotPokeApiResponse> pokemonAndAbilityNamesPair in pokemonAndAbilityNamesPairs)
        {
            if (existingPokemonMap.TryGetValue(pokemonAndAbilityNamesPair.Key, out Pokemon pokemon))
            {
                if (existingAbilitiesMap.TryGetValue(pokemonAndAbilityNamesPair.Value.AbilityInfo.Name, out Ability ability))
                {
                    if (!newAbilitiesInPokemon.Any(x => IsEqual(x, ability.Id, pokemon.Id, pokemonAndAbilityNamesPair.Value.IsHidden)))
                    {
                        newAbilitiesInPokemon.Add(new AbilityInPokemon
                        {
                            AbilityId = ability.Id,
                            PokemonId = pokemon.Id,
                            IsHidden = pokemonAndAbilityNamesPair.Value.IsHidden
                        });
                    }
                }
            }
        }

        await _unitOfWork.AbilityInPokemonDao.CreateRangeAsync(newAbilitiesInPokemon);
        await _unitOfWork.CommitAsync();
    }

    private async Task CreateMovesInPokemon(ICollection<PokemonPokeApiResponse> pokemonResponses)
    {
        IEnumerable<KeyValuePair<string, string>> pokemonAndMoveNamesPairs = 
            from p in pokemonResponses
            from m in p.Moves
            select new KeyValuePair<string, string>(p.Name, m.MoveUriPokeApiResponse.Name);

        IDictionary<string, Pokemon> existingPokemonMap = await _unitOfWork.PokemonDao.LoadMapByName();
        IDictionary<string, Move> existingMovesMap = await  _unitOfWork.MoveDao.LoadMapByName();
        
        List<MoveInPokemon> newMovesInPokemon = [];
        foreach (KeyValuePair<string, string> pokemonAndMoveNamesPair in pokemonAndMoveNamesPairs)
        {
            if (existingPokemonMap.TryGetValue(pokemonAndMoveNamesPair.Key, out Pokemon pokemon))
            {
                if (existingMovesMap.TryGetValue(pokemonAndMoveNamesPair.Value, out Move move))
                {
                    if (!newMovesInPokemon.Any(x => IsEqual(x, move.Id, pokemon.Id)))
                    {
                        newMovesInPokemon.Add(new MoveInPokemon
                        {
                            MoveId = move.Id,
                            PokemonId = pokemon.Id
                        });
                    }
                }
            }
        }

        await _unitOfWork.MoveInPokemonDao.CreateRangeAsync(newMovesInPokemon);
        await _unitOfWork.CommitAsync();
    }

    private static bool IsEqual(AbilityInPokemon abilityInPokemon, int abilityId, int pokemonId, bool isHidden)
    {
        return abilityInPokemon.AbilityId == abilityId &&
            abilityInPokemon.PokemonId == pokemonId &&
            abilityInPokemon.IsHidden == isHidden;
    }

    private static bool IsEqual(MoveInPokemon moveInPokemon, int moveId, int pokemonId)
    {
        return moveInPokemon.MoveId == moveId &&
            moveInPokemon.PokemonId == pokemonId;
    }
}
