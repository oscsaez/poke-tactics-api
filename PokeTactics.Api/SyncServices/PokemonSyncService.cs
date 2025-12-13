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

    public PokemonSyncService(IUnitOfWork unitOfWork, ILogger<AbilitySyncService> logger, IHttpClientFactory httpClientFactory)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient(ApiConstants.ExternalApiName);
    }

    public async Task Sync(CancellationToken cancellationToken)
    {
        IDictionary<string, Pokemon> existingPokemonMap = await _unitOfWork.PokemonDao.LoadMapByName();
        IDictionary<string, Ability> existingAbilitiesMap = await _unitOfWork.AbilityDao.LoadMapByName();
        IDictionary<string, Move> existingMovesMap = await _unitOfWork.MoveDao.LoadMapByName();

        PokemonSummaryListPokeApiResponse allPokemonResponse = await GetPokemonSummaryListFromApi(cancellationToken);

        ConcurrentBag<Pokemon> newPokemonConcurrentList = [];
        ConcurrentBag<Pokemon> updatedPokemonConcurrentList = [];
        ConcurrentBag<PokemonPokeApiResponse> newPokemonConcurrentResponses = [];

        ParallelOptions parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = ApiConstants.NumberOfRequestsGrantedConcurrently,
            CancellationToken = cancellationToken
        };

        var allPokemonNames = allPokemonResponse.Results.Select(x => x.Name);
        var pokemonNamesToDelete = existingAbilitiesMap.Select(x => x.Key).Except(allPokemonNames);

        await _unitOfWork.PokemonDao.DeleteByNames(pokemonNamesToDelete);

        await Parallel.ForEachAsync(allPokemonResponse.Results, parallelOptions, async (pokemonSummary, ct) =>
        {
            try
            {
                PokemonPokeApiResponse pokemonInfoResponse = await GetPokemonFromApi(pokemonSummary, ct);

                if (existingPokemonMap.TryGetValue(pokemonSummary.Name, out Pokemon existingPokemon))
                {
                    Pokemon pokemonFromResponse = pokemonInfoResponse.ToPokemonWithAbilitiesAndMoves();

                    if (!existingPokemon.Compare(pokemonFromResponse))
                    {
                        existingPokemon.MapExisting(pokemonFromResponse, existingAbilitiesMap, existingMovesMap);
                        updatedPokemonConcurrentList.Add(existingPokemon);
                    }
                }
                else
                {
                    newPokemonConcurrentList.Add(pokemonInfoResponse.ToPokemon());
                    newPokemonConcurrentResponses.Add(pokemonInfoResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing pokemon {}", pokemonSummary.Name);
            }
        });

        await _unitOfWork.PokemonDao.CreateRangeAsync(newPokemonConcurrentList.ToList());
        await _unitOfWork.PokemonDao.UpdateRangeAsync(updatedPokemonConcurrentList.ToList());

        await _unitOfWork.CommitAsync();

        if (!newPokemonConcurrentList.IsNullOrEmpty())
        {
            List<PokemonPokeApiResponse> newPokemonResponses = newPokemonConcurrentResponses.ToList();
            existingPokemonMap = await _unitOfWork.PokemonDao.LoadMapByName();

            await CreateAbilitiesInPokemon(newPokemonResponses, existingPokemonMap, existingAbilitiesMap);
            await CreateMovesInPokemon(newPokemonResponses, existingPokemonMap, existingMovesMap);
        }

        await _unitOfWork.CommitAsync();

        _logger.LogInformation("Creation/Update of pokemon has been done!");
    }

    private async Task<PokemonSummaryListPokeApiResponse> GetPokemonSummaryListFromApi(CancellationToken cancellationToken)
    {
        return await _httpClient.GetFromJsonAsync<PokemonSummaryListPokeApiResponse>(ApiConstants.AllPokemonInfoUri, cancellationToken)
            ?? throw new PokeApiSyncException(MessagesHelper.GetApiCallFailMessage(ApiConstants.AllPokemonInfoUri));
    }

    private async Task<PokemonPokeApiResponse> GetPokemonFromApi(PokemonSummaryPokeApiResponse pokemonSummary, CancellationToken ct)
    {
        return await _httpClient.GetFromJsonAsync<PokemonPokeApiResponse>(pokemonSummary.Url, ct)
            ?? throw new PokeApiSyncException(MessagesHelper.GetApiCallFailMessage(pokemonSummary.Url));
    }

    private async Task CreateAbilitiesInPokemon(
        ICollection<PokemonPokeApiResponse> pokemonResponses,
        IDictionary<string, Pokemon> existingPokemonMap,
        IDictionary<string, Ability> existingAbilitiesMap)
    {
        IEnumerable<KeyValuePair<string, AbilitySlotPokeApiResponse>> pokemonAndAbilityNamesPairs = pokemonResponses
            .SelectMany(p => p.Abilities.Select(a => new KeyValuePair<string, AbilitySlotPokeApiResponse>(p.Name, a)));
        
        List<AbilityInPokemon> newAbilitiesInPokemon = [];
        HashSet<AbilityInPokemonKey> abilitiesInPokemonSeen = [];

        foreach (KeyValuePair<string, AbilitySlotPokeApiResponse> pokemonAndAbilityNamesPair in pokemonAndAbilityNamesPairs)
        {
            if (!existingPokemonMap.TryGetValue(pokemonAndAbilityNamesPair.Key, out Pokemon pokemon))
            {
                continue;
            }

            if (!existingAbilitiesMap.TryGetValue(pokemonAndAbilityNamesPair.Value.AbilityInfo.Name, out Ability ability))
            {
                continue;
            }

            AbilityInPokemonKey key = new(pokemon.Id, ability.Id, pokemonAndAbilityNamesPair.Value.IsHidden);

            if (!abilitiesInPokemonSeen.Add(key))
            {
                continue;
            }

            newAbilitiesInPokemon.Add(new AbilityInPokemon
            {
                AbilityId = ability.Id,
                PokemonId = pokemon.Id,
                IsHidden = pokemonAndAbilityNamesPair.Value.IsHidden
            });
        }

        await _unitOfWork.AbilityInPokemonDao.CreateRangeAsync(newAbilitiesInPokemon);
    }

    private async Task CreateMovesInPokemon(
        ICollection<PokemonPokeApiResponse> pokemonResponses,
        IDictionary<string, Pokemon> existingPokemonMap,
        IDictionary<string, Move> existingMovesMap)
    {
        IEnumerable<KeyValuePair<string, string>> pokemonAndMoveNamesPairs = pokemonResponses
            .SelectMany(p => p.Moves.Select(m => new KeyValuePair<string, string>(p.Name, m.MoveUriPokeApiResponse.Name)));
        
        List<MoveInPokemon> newMovesInPokemon = [];
        HashSet<MoveInPokemonKey> movesInPokemonSeen = []; 
        foreach (KeyValuePair<string, string> pokemonAndMoveNamesPair in pokemonAndMoveNamesPairs)
        {
            if (!existingPokemonMap.TryGetValue(pokemonAndMoveNamesPair.Key, out Pokemon pokemon))
            {
                continue;
            }

            if (!existingMovesMap.TryGetValue(pokemonAndMoveNamesPair.Value, out Move move))
            {
                continue;
            }

            MoveInPokemonKey key = new(pokemon.Id, move.Id);

            if (!movesInPokemonSeen.Add(key))
            {
                continue;
            }

            newMovesInPokemon.Add(new MoveInPokemon
            {
                MoveId = move.Id,
                PokemonId = pokemon.Id
            });
        }

        await _unitOfWork.MoveInPokemonDao.CreateRangeAsync(newMovesInPokemon);
    }

    private readonly record struct AbilityInPokemonKey(int PokemonId, int AbilityId, bool IsHidden);
    private readonly record struct MoveInPokemonKey(int PokemonId, int MoveId);
}
