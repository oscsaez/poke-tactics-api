using System.Text.Json.Serialization;
using PokeTactics.Contracts.Constants;
using PokeTactics.Contracts.EffectEntry.PokeApi;
using PokeTactics.Contracts.Type.PokeApi;

namespace PokeTactics.Contracts.Move.PokeApi;

public class MoveInfoPokeApiResponse
{
    public int? Power { get; set; }

    public int? Accuracy { get; set; }

    public int? Pp { get; set; }

    [JsonPropertyName(ContractsConstants.EffectEntriesPokeApiJsonPropertyName)]
    public required ICollection<EffectEntryPokeApiResponse> EffectEntries { get; set; }

    public required TypeInfoPokeApiResponse Type { get; set; }
}
