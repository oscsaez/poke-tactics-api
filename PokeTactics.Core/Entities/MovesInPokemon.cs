namespace PokeTactics.Core.Entities
{
    public class MovesInPokemon
    {
        public int PokemonId { get; set; }

        public Pokemon Pokemon { get; set; } = default!;

        public int MoveId { get; set; }

        public Move Move { get; set; } = default!;
    }
}