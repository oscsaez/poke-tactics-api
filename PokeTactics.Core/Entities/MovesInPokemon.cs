namespace PokeTactics.Core.Entities
{
    // TODO: Rename this class to MoveInPokemon
    public class MovesInPokemon
    {
        public int PokemonId { get; set; }

        public Pokemon Pokemon { get; set; } = default!;

        public int MoveId { get; set; }

        public Move Move { get; set; } = default!;
    }
}