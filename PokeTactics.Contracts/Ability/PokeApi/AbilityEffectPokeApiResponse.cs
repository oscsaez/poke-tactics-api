using System.Text.Json.Serialization;
using PokeTactics.Contracts.Constants;
using PokeTactics.Contracts.Move.PokeApi;

namespace PokeTactics.Contracts.Ability.PokeApi;

public class AbilityEffectPokeApiResponse
{
    [JsonPropertyName(SharedConstants.EffectEntriesPokeApiJsonPropertyName)]
    public required ICollection<EffectEntryPokeApiResponse> EffectEntries { get; set; }
}
