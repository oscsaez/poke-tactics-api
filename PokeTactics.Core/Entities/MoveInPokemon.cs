namespace PokeTactics.Core.Entities
{
    public class MoveInPokemon : Entity
    {
        public int PokemonId { get; set; }

        public Pokemon Pokemon { get; set; } = default!;

        public int MoveId { get; set; }

        public Move Move { get; set; } = default!;
    }
}