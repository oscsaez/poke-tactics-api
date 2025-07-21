using System.ComponentModel.DataAnnotations;
using PokeTactics.Core.Utils;

namespace PokeTactics.Core.Entities
{
    public class Move : Entity
    {
        [MaxLength(CoreConstants.MaxMoveNameLength)]
        public required string Name { get; set; }

        public int? Power { get; set; }

        public int? Accuracy { get; set; }

        public int PowerPoints { get; set; }

        [MaxLength(CoreConstants.MaxDescriptionLength)]
        public string? Description { get; set; }

        [MaxLength(CoreConstants.MaxTypeNameLength)]
        public required string Type { get; set; }

        public ICollection<MovesInPokemon> MovesInPokemon { get; set; } = [];
    }
}