
using System.Collections.Concurrent;
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
            await SyncAbilities(cancellationToken);
            await SyncMoves(cancellationToken);
            await SyncPokemon(cancellationToken);
        }

        // This method and SyncMoves are equal in structure, but if doing a template method for them will be problematic if in a future the
        // structure of the response of pokeapi changes between abilities and moves
        private async Task SyncAbilities(CancellationToken cancellationToken)
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

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
        private async Task SyncMoves(CancellationToken cancellationToken)
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

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

        private async Task SyncPokemon(CancellationToken cancellationToken)
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            IDictionary<string, Pokemon> existingPokemonMap = await unitOfWork.PokemonDao.LoadMapByName();
            IDictionary<string, Ability> existingAbilitiesMap = await unitOfWork.AbilityDao.LoadMapByName();
            IDictionary<string, Move> existingMovesMap = await unitOfWork.MoveDao.LoadMapByName();

            PokemonSummaryListPokeApiResponse allPokemonResponse =
                await _httpClient.GetFromJsonAsync<PokemonSummaryListPokeApiResponse>(ApiConstants.AllPokemonInfoUri, cancellationToken)
                ?? throw new PokeApiSyncException(GetApiCallFailMessage(ApiConstants.AllPokemonInfoUri));

            ConcurrentBag<Pokemon> newPokemonList = [];
            ConcurrentBag<Pokemon> updatedPokemonList = [];
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

                        Pokemon pokemonFromResponse = pokemonInfoResponse.ToPokemon();

                        if (existingPokemonMap.TryGetValue(pokemonSummary.Name, out Pokemon existingPokemon))
                        {
                            if (existingPokemon.Compare(pokemonFromResponse))
                            {
                                return;
                            }

                            existingPokemon.MapExisting(pokemonFromResponse, existingAbilitiesMap, existingMovesMap);
                            updatedPokemonList.Add(existingPokemon);
                        }
                        else
                        {
                            FillAbilities(pokemonFromResponse, existingAbilitiesMap);
                            FillMoves(pokemonFromResponse, existingMovesMap);
                            newPokemonList.Add(pokemonFromResponse);
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

            await unitOfWork.PokemonDao.CreateRangeAsync(newPokemonList.ToList());
            await unitOfWork.PokemonDao.UpdateRangeAsync(updatedPokemonList.ToList());
            await unitOfWork.CommitAsync();

            _logger.LogInformation("Creation/Update of pokemon has been done!");
        }

        private static void FillAbilities(Pokemon pokemon, IDictionary<string, Ability> abilitiesMap)
        {
            foreach (AbilitiesInPokemon abilityInPokemon in pokemon.AbilitiesInPokemon)
            {
                if (abilitiesMap.TryGetValue(abilityInPokemon.Ability.Name, out Ability ability))
                {
                    abilityInPokemon.Ability = ability;
                }
            }
        }

        private static void FillMoves(Pokemon pokemon, IDictionary<string, Move> movesMap)
        {
            foreach (MovesInPokemon moveInPokemon in pokemon.MovesInPokemon)
            {
                if (movesMap.TryGetValue(moveInPokemon.Move.Name, out Move move))
                {
                    moveInPokemon.Move = move;
                }
            }
        }

        private static string GetApiCallFailMessage(string path)
        {
            return $"Something failed calling PokeApi. Path: [{path}].";
        }
    }
}