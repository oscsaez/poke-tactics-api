using System.Text.Json.Serialization;
using PokeTactics.Contracts.Constants;
using PokeTactics.Contracts.EffectEntry.PokeApi;

namespace PokeTactics.Contracts.Ability.PokeApi;

public class AbilityEffectPokeApiResponse
{
    [JsonPropertyName(ContractsConstants.EffectEntriesPokeApiJsonPropertyName)]
    public required ICollection<EffectEntryPokeApiResponse> EffectEntries { get; set; }
}
