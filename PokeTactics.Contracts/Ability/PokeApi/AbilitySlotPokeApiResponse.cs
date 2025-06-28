namespace PokeTactics.Contracts.Ability.PokeApi
{
    public class AbilitySlotPokeApiResponse
    {
        public required AbilitySummaryPokeApiResponse Ability { get; set; }

        public bool IsHidden { get; set; }

        public int Slot { get; set; }
    }
}