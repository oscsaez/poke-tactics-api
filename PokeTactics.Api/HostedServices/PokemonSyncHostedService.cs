
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
            IOptions<PokemonSyncSettings> options)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _httpClient = new HttpClient { BaseAddress = ApiConstants.AllPokemonInfoUri.ToUri() };
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

            // TODO Finish versions comparison and pokemon update
            int numberOfPokemonInDatabase = await unitOfWork.PokemonDao.CountAsync();

            PokemonSummaryListPokeApiResponse response = await _httpClient.GetFromJsonAsync<PokemonSummaryListPokeApiResponse>(ApiConstants.AllPokemonInfoUri, cancellationToken)
                ?? throw new PokeApiSyncException($"Something failed calling PokeApi. Path: [{ApiConstants.AllPokemonInfoUri}]");

            if (numberOfPokemonInDatabase != response.Count)
            {
                _logger.LogInformation("New Pokemon available in PokeApi. Initializing synchronization...");
                await SyncAllPokemon(unitOfWork, response, cancellationToken);
            }
            else
            {
                _logger.LogInformation("No new Pokemon available in PokeApi.");
            }

            _logger.LogInformation("Pokemon synchronization completed.");
        }

        private async Task SyncAllPokemon(IUnitOfWork unitOfWork, PokemonSummaryListPokeApiResponse response, CancellationToken cancellationToken)
        {
            IEnumerable<string> pokemonUris = response.Results.Select(x => x.Url);

            foreach (string pokemonUri in pokemonUris)
            {
                await _semaphore.WaitAsync(cancellationToken);

                _ = Task.Run(async () =>
                {
                    try
                    {
                        PokemonPokeApiResponse pokemonResponse = await _httpClient.GetFromJsonAsync<PokemonPokeApiResponse>(pokemonUri, cancellationToken)
                            ?? throw new PokeApiSyncException($"Something failed calling PokeApi. Path: [{pokemonUri}]");

                        Pokemon pokemon = pokemonResponse.ToPokemon();

                        ICollection<MovesInPokemon> moves = await GetMovesInPokemon(pokemonResponse.Moves, cancellationToken);
                        ICollection<AbilitiesInPokemon> abilities = await GetAbilitiesInPokemon(pokemonResponse.Abilities, cancellationToken);

                        pokemon.AbilitiesInPokemon.AddRange(abilities);
                        pokemon.MovesInPokemon.AddRange(moves);

                        // TODO IMPORTANT -> Add PokeApi Ids of moves, abilities, etc and some logic not to trying to insert
                        // duplicates on DB
                        await unitOfWork.PokemonDao.CreateAsync(pokemon);
                        await unitOfWork.CommitAsync();
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception, "Error while trying to map and add a new Pokemon to DB.");
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                }, cancellationToken);
            }
        }

        private async Task<ICollection<MovesInPokemon>> GetMovesInPokemon(ICollection<MovePokeApiResponse> moveResponses, CancellationToken cancellationToken)
        {
            IEnumerable<MoveUriPokeApiResponse> moveUriResponses = moveResponses.Select(x => x.MoveUriPokeApiResponse);
            ICollection<MovesInPokemon> movesInPokemon = [];

            foreach (MoveUriPokeApiResponse moveUriResponse in moveUriResponses)
            {
                string moveUri = moveUriResponse.Url;

                MoveInfoPokeApiResponse moveInfoResponse = await _httpClient.GetFromJsonAsync<MoveInfoPokeApiResponse>(moveUri, cancellationToken)
                    ?? throw new PokeApiSyncException($"Something failed calling PokeApi. Path: [{moveUri}]");

                movesInPokemon.Add(MovesInPokemonMapper.MovePokeApiResponseToMovesInPokemon(moveUriResponse.Name, moveInfoResponse));
            }

            return movesInPokemon;
        }

        private async Task<ICollection<AbilitiesInPokemon>> GetAbilitiesInPokemon(
            ICollection<AbilitySlotPokeApiResponse> abilityResponses,
            CancellationToken cancellationToken)
        {
            ICollection<AbilitiesInPokemon> abilitiesInPokemon = [];

            foreach (AbilitySlotPokeApiResponse abilityResponse in abilityResponses)
            {
                string abilityUri = abilityResponse.AbilityInfo.Url;

                AbilityEffectPokeApiResponse abilityEffectResponse = await _httpClient.GetFromJsonAsync<AbilityEffectPokeApiResponse>(abilityUri, cancellationToken)
                    ?? throw new PokeApiSyncException($"Something failed calling PokeApi. Path: [{abilityUri}]");

                abilitiesInPokemon.Add(AbilitiesInPokemonMapper.AbilityPokeApiResponseToAbilitiesInPokemon(abilityResponse, abilityEffectResponse));
            }

            return abilitiesInPokemon;
        }
    }
}