
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PokeTactics.Api.Utils;
using PokeTactics.Contracts.Ability.PokeApi;
using PokeTactics.Contracts.Move.PokeApi;
using PokeTactics.Contracts.Pokemon.PokeApi;
using PokeTactics.Core.Definitions;
using PokeTactics.Core.Entities;
using PokeTactics.Core.Exceptions;
using PokeTactics.Core.Interfaces;
using PokeTactics.Core.Utils.Extensions;
using PokeTactics.Services.Mappers;

namespace PokeTactics.Api.HostedServices
{
    public class PokemonSyncHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PokemonSyncHostedService> _logger;
        private readonly HttpClient _httpClient;
        private readonly SemaphoreSlim _semaphore;
        private readonly int _intervalMinutes;

        public PokemonSyncHostedService(
            IServiceProvider serviceProvider,
            ILogger<PokemonSyncHostedService> logger,
            IHttpClientFactory httpClientFactory,
            IOptions<PokemonSyncSettings> options)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient(ApiConstants.ExternalApiName);
            _semaphore = new SemaphoreSlim(5);
            _intervalMinutes = options.Value.IntervalMinutes;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Pokemon synchronization service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await SyncIfNeeded(stoppingToken);

                    // Wait until next synchronization
                    await Task.Delay(TimeSpan.FromMinutes(_intervalMinutes), stoppingToken);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Error while synchronizing Pokemon.");
                }
            }
        }

        private async Task SyncIfNeeded(CancellationToken cancellationToken)
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            await SyncAbilities(unitOfWork, cancellationToken);
            await SyncMoves(unitOfWork, cancellationToken);
            await SyncPokemon(unitOfWork, cancellationToken);
        }

        // This method and SyncMoves are equal in structure, but if doing a template method for them will be problematic if in a future the
        // structure of the response of pokeapi changes between abilities and moves
        private async Task SyncAbilities(IUnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            IDictionary<string, Ability> existingAbilitiesMap = await unitOfWork.AbilityDao.LoadMapByName();

            AbilitySummaryListPokeApiResponse allAbilitiesResponse =
                await _httpClient.GetFromJsonAsync<AbilitySummaryListPokeApiResponse>(ApiConstants.AllAbilitiesInfoUri, cancellationToken)
                ?? throw new PokeApiSyncException(GetApiCallFailMessage(ApiConstants.AllAbilitiesInfoUri));

            ConcurrentBag<Ability> newAbilities = [];
            ConcurrentBag<Ability> updatedAbilities = [];
            List<Task> apiCalls = [];

            foreach (AbilitySummaryPokeApiResponse abilitySummary in allAbilitiesResponse.Results)
            {
                await _semaphore.WaitAsync(cancellationToken);

                apiCalls.Add(Task.Run(async () =>
                {
                    try
                    {
                        AbilityEffectPokeApiResponse abilityEffectEntries =
                            await _httpClient.GetFromJsonAsync<AbilityEffectPokeApiResponse>(abilitySummary.Url, cancellationToken)
                            ?? throw new PokeApiSyncException(GetApiCallFailMessage(abilitySummary.Url));

                        Ability abilityFromResponse = abilityEffectEntries.ToAbility(abilitySummary.Name);

                        if (existingAbilitiesMap.TryGetValue(abilitySummary.Name, out Ability existingAbility))
                        {
                            if (existingAbility.Compare(abilityFromResponse))
                            {
                                return;
                            }

                            existingAbility.MapExisting(abilityFromResponse);
                            updatedAbilities.Add(existingAbility);
                        }
                        else
                        {
                            newAbilities.Add(abilityFromResponse);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error while trying to get new abilities or the ones to be updated.");
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                }, cancellationToken));
            }

            await Task.WhenAll(apiCalls);

            await unitOfWork.AbilityDao.CreateRangeAsync(newAbilities);
            await unitOfWork.AbilityDao.UpdateRangeAsync(updatedAbilities);
            await unitOfWork.CommitAsync();

            _logger.LogInformation("Creation/Update of abilities has been done!");
        }

        // This method and SyncAbilities are equal in structure, but if doing a template method for them will be problematic if in a future the
        // structure of the response of pokeapi changes between abilities and moves
        private async Task SyncMoves(IUnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            IDictionary<string, Move> existingMovesMap = await unitOfWork.MoveDao.LoadMapByName();

            MoveSummaryListPokeApiResponse allMovesResponse =
                await _httpClient.GetFromJsonAsync<MoveSummaryListPokeApiResponse>(ApiConstants.AllMovesInfoUri, cancellationToken)
                ?? throw new PokeApiSyncException(GetApiCallFailMessage(ApiConstants.AllMovesInfoUri));

            ConcurrentBag<Move> newMoves = [];
            ConcurrentBag<Move> updatedMoves = [];
            List<Task> apiCalls = [];

            foreach (MoveSummaryPokeApiResponse moveSummary in allMovesResponse.Results)
            {
                await _semaphore.WaitAsync(cancellationToken);

                apiCalls.Add(Task.Run(async () =>
                {
                    try
                    {
                        MoveInfoPokeApiResponse moveInfoResponse = await _httpClient.GetFromJsonAsync<MoveInfoPokeApiResponse>(moveSummary.Url, cancellationToken)
                            ?? throw new PokeApiSyncException(GetApiCallFailMessage(moveSummary.Url));

                        Move moveFromResponse = moveInfoResponse.ToMove(moveSummary.Name);

                        if (existingMovesMap.TryGetValue(moveSummary.Name, out Move existingMove))
                        {
                            if (existingMove.Compare(moveFromResponse))
                            {
                                return;
                            }

                            existingMove.MapExisting(moveFromResponse);
                            updatedMoves.Add(existingMove);
                        }
                        else
                        {
                            newMoves.Add(moveFromResponse);
                        }
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception, "Error while trying to get new moves or the ones to be updated.");
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                }, cancellationToken));
            }

            await Task.WhenAll(apiCalls);

            await unitOfWork.MoveDao.CreateRangeAsync(newMoves.ToList());
            await unitOfWork.MoveDao.UpdateRangeAsync(updatedMoves.ToList());
            await unitOfWork.CommitAsync();

            _logger.LogInformation("Creation/Update of moves has been done!");
        }

        private async Task SyncPokemon(IUnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            IDictionary<string, Pokemon> existingPokemonMap = await unitOfWork.PokemonDao.LoadMapByName();
            IDictionary<string, Ability> existingAbilitiesMap = await unitOfWork.AbilityDao.LoadMapByName();
            IDictionary<string, Move> existingMovesMap = await unitOfWork.MoveDao.LoadMapByName();

            PokemonSummaryListPokeApiResponse allPokemonResponse =
                await _httpClient.GetFromJsonAsync<PokemonSummaryListPokeApiResponse>(ApiConstants.AllPokemonInfoUri, cancellationToken)
                ?? throw new PokeApiSyncException(GetApiCallFailMessage(ApiConstants.AllPokemonInfoUri));

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
                            ?? throw new PokeApiSyncException(GetApiCallFailMessage(pokemonSummary.Url));

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

            await unitOfWork.PokemonDao.CreateRangeAsync(newPokemonConcurrentList.ToList());
            await unitOfWork.PokemonDao.UpdateRangeAsync(updatedPokemonConcurrentList.ToList());
            await unitOfWork.CommitAsync();

            if(!newPokemonConcurrentList.IsNullOrEmpty())
            {
                List<PokemonPokeApiResponse> newPokemonResponses = newPokemonConcurrentResponses.ToList();

                await CreateAbilitiesInPokemon(newPokemonResponses, unitOfWork);
                await CreateMovesInPokemon(newPokemonResponses, unitOfWork);
            }

            _logger.LogInformation("Creation/Update of pokemon has been done!");
        }

        private async Task CreateAbilitiesInPokemon(ICollection<PokemonPokeApiResponse> pokemonResponses, IUnitOfWork unitOfWork)
        {
            IEnumerable<KeyValuePair<string, AbilitySlotPokeApiResponse>> pokemonAndAbilityNamesPairs = 
                from p in pokemonResponses
                from a in p.Abilities
                select new KeyValuePair<string, AbilitySlotPokeApiResponse>(p.Name, a);

            IDictionary<string, Pokemon> existingPokemonMap = await unitOfWork.PokemonDao.LoadMapByName();
            IDictionary<string, Ability> existingAbilitiesMap = await  unitOfWork.AbilityDao.LoadMapByName();
            
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

            await unitOfWork.AbilityInPokemonDao.CreateRangeAsync(newAbilitiesInPokemon);
            await unitOfWork.CommitAsync();
        }

        private async Task CreateMovesInPokemon(ICollection<PokemonPokeApiResponse> pokemonResponses, IUnitOfWork unitOfWork)
        {
            IEnumerable<KeyValuePair<string, string>> pokemonAndMoveNamesPairs = 
                from p in pokemonResponses
                from m in p.Moves
                select new KeyValuePair<string, string>(p.Name, m.MoveUriPokeApiResponse.Name);

            IDictionary<string, Pokemon> existingPokemonMap = await unitOfWork.PokemonDao.LoadMapByName();
            IDictionary<string, Move> existingMovesMap = await  unitOfWork.MoveDao.LoadMapByName();
            
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

            await unitOfWork.MoveInPokemonDao.CreateRangeAsync(newMovesInPokemon);
            await unitOfWork.CommitAsync();
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

        private static string GetApiCallFailMessage(string path)
        {
            return $"Something failed calling PokeApi. Path: [{path}].";
        }
    }
}