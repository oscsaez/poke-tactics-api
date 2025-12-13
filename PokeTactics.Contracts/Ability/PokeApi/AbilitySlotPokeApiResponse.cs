using System.Text.Json.Serialization;

namespace PokeTactics.Contracts.Ability.PokeApi
{
    public class AbilitySlotPokeApiResponse
    {
        [JsonPropertyName("ability")]
        public required AbilitySummaryPokeApiResponse AbilityInfo { get; set; }

        public bool IsHidden { get; set; }

        public int Slot { get; set; }
    }
}