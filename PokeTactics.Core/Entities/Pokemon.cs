using System.ComponentModel.DataAnnotations;
using PokeTactics.Core.Utils;
using PokeTactics.Core.Utils.Attributes;

namespace PokeTactics.Core.Entities
{
    public class Pokemon : Entity
    {
        public int? PokedexOrder { get; set; }

        [MaxLength(CoreConstants.MaxPokemonNameLength)]
        public required string Name { get; set; }

        public double Height { get; set; }

        public double Weight { get; set; }

        [MaxListCount(CoreConstants.MaxTypesListCount)]
        public required ICollection<string> Types { get; set; }

        [MaxListCount(CoreConstants.MaxStatsListCount)]
        public required ICollection<Stat> Stats { get; set; }

        [MaxListCount(CoreConstants.MaxAbilitiesListCount)]
        public required ICollection<Ability> Abilities { get; set; }

        public required ICollection<Move> Moves { get; set; }

        public required Sprite Sprite { get; set; }
    }
}