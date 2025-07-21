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
        public ICollection<string> Types { get; set; } = [];

        // TODO Is this necessary? Spike different stats as properties
        [MaxListCount(CoreConstants.MaxStatsListCount)]
        public ICollection<Stat> Stats { get; set; } = [];

        [MaxListCount(CoreConstants.MaxAbilitiesListCount)]
        public ICollection<AbilitiesInPokemon> AbilitiesInPokemon { get; set; } = [];

        public ICollection<MovesInPokemon> MovesInPokemon { get; set; } = [];

        public required Sprite Sprite { get; set; }
    }
}