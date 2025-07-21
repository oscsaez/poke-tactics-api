
using PokeTactics.Api.Utils;
using PokeTactics.Contracts.Move.PokeApi;
using PokeTactics.Contracts.Pokemon.PokeApi;
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

        public PokemonSyncHostedService(IServiceProvider serviceProvider, ILogger<PokemonSyncHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _httpClient = new HttpClient { BaseAddress = ApiConstants.AllPokemonInfoUri.ToUri() };
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Pokemon synchronization service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                // TODO Call SyncIfNeeded...
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

            if (numberOfPokemonInDatabase > response.Count)
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
                PokemonPokeApiResponse pokemonResponse = await _httpClient.GetFromJsonAsync<PokemonPokeApiResponse>(pokemonUri, cancellationToken)
                    ?? throw new PokeApiSyncException($"Something failed calling PokeApi. Path: [{pokemonUri}]");

                Pokemon pokemon = pokemonResponse.ToPokemon();

                ICollection<MovesInPokemon> moves = await GetMovesInPokemon(pokemonResponse.Moves, cancellationToken);

                // TODO Continue with filling abilities and IMPORTANT -> Add PokeApi Ids of moves, abilities, etc and some logic not to trying to insert
                // duplicates on DB
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
    }
}