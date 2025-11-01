namespace PokeTactics.Core.Entities
{
    public class AbilitiesInPokemon
    {
        public int PokemonId { get; set; }

        public Pokemon Pokemon { get; set; } = default!;

        public int AbilityId { get; set; }

        public Ability Ability { get; set; } = default!;

        public bool IsHidden { get; set; }
    }
}