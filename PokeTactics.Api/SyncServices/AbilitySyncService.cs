using System.Collections.Concurrent;
using PokeTactics.Api.Utils;
using PokeTactics.Contracts.Ability.PokeApi;
using PokeTactics.Core.Entities;
using PokeTactics.Core.Exceptions;
using PokeTactics.Core.Interfaces;
using PokeTactics.Core.Interfaces.SyncServices;
using PokeTactics.Core.Utils.Extensions;
using PokeTactics.Services.Mappers;

namespace PokeTactics.Api.SyncServices;

public class AbilitySyncService : IAbilitySyncService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AbilitySyncService> _logger;
    private readonly HttpClient _httpClient;

    public AbilitySyncService(IUnitOfWork unitOfWork, ILogger<AbilitySyncService> logger, IHttpClientFactory httpClientFactory)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient(ApiConstants.ExternalApiName);
    }
    
    // This method and MoveSyncService.Sync are equal in structure, but if doing a template method for them will be problematic if in a future the
    // structure of the response of pokeapi changes between abilities and moves
    public async Task Sync(CancellationToken cancellationToken)
    {
        IDictionary<string, Ability> existingAbilitiesMap = await _unitOfWork.AbilityDao.LoadMapByName();

        AbilitySummaryListPokeApiResponse allAbilitiesResponse =
            await _httpClient.GetFromJsonAsync<AbilitySummaryListPokeApiResponse>(ApiConstants.AllAbilitiesInfoUri, cancellationToken)
            ?? throw new PokeApiSyncException(MessagesHelper.GetApiCallFailMessage(ApiConstants.AllAbilitiesInfoUri));

        ConcurrentBag<Ability> newAbilities = [];
        ConcurrentBag<Ability> updatedAbilities = [];
        
        ParallelOptions parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = ApiConstants.NumberOfRequestsGrantedConcurrently,
            CancellationToken = cancellationToken
        };

        await Parallel.ForEachAsync(allAbilitiesResponse.Results, parallelOptions, async (abilitySummary, ct) =>
        {
            try
                {
                    AbilityEffectPokeApiResponse abilityEffectEntries =
                        await _httpClient.GetFromJsonAsync<AbilityEffectPokeApiResponse>(abilitySummary.Url, ct)
                        ?? throw new PokeApiSyncException(MessagesHelper.GetApiCallFailMessage(abilitySummary.Url));

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
        });

        await _unitOfWork.AbilityDao.CreateRangeAsync(newAbilities);
        await _unitOfWork.AbilityDao.UpdateRangeAsync(updatedAbilities);
        await _unitOfWork.CommitAsync();

        _logger.LogInformation("Creation/Update of abilities has been done!");
    }
}
