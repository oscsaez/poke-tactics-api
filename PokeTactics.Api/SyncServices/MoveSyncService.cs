using System.Collections.Concurrent;
using PokeTactics.Api.Utils;
using PokeTactics.Contracts.Move.PokeApi;
using PokeTactics.Core.Entities;
using PokeTactics.Core.Exceptions;
using PokeTactics.Core.Interfaces;
using PokeTactics.Core.Interfaces.SyncServices;
using PokeTactics.Core.Utils.Extensions;
using PokeTactics.Services.Mappers;

namespace PokeTactics.Api.SyncServices;

public class MoveSyncService : IMoveSyncService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MoveSyncService> _logger;
    private readonly HttpClient _httpClient;
    private readonly SemaphoreSlim _semaphore;

    public MoveSyncService(IUnitOfWork unitOfWork, ILogger<MoveSyncService> logger, IHttpClientFactory httpClientFactory)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient(ApiConstants.ExternalApiName);
        _semaphore = new SemaphoreSlim(ApiConstants.NumberOfRequestsGrantedConcurrently);
    }

    // This method and AbilitySyncService.Sync are equal in structure, but if doing a template method for them will be problematic if in a future the
    // structure of the response of pokeapi changes between abilities and moves
    public async Task Sync(CancellationToken cancellationToken)
    {
        IDictionary<string, Move> existingMovesMap = await _unitOfWork.MoveDao.LoadMapByName();

        MoveSummaryListPokeApiResponse allMovesResponse =
            await _httpClient.GetFromJsonAsync<MoveSummaryListPokeApiResponse>(ApiConstants.AllMovesInfoUri, cancellationToken)
            ?? throw new PokeApiSyncException(MessagesHelper.GetApiCallFailMessage(ApiConstants.AllMovesInfoUri));

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
                        ?? throw new PokeApiSyncException(MessagesHelper.GetApiCallFailMessage(moveSummary.Url));

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

        await _unitOfWork.MoveDao.CreateRangeAsync(newMoves.ToList());
        await _unitOfWork.MoveDao.UpdateRangeAsync(updatedMoves.ToList());
        await _unitOfWork.CommitAsync();

        _logger.LogInformation("Creation/Update of moves has been done!");
    }
}
