using System.ComponentModel.DataAnnotations;
using PokeTactics.Core.Utils;

namespace PokeTactics.Core.Entities
{
    public class Stat : Entity
    {
        [MaxLength(CoreConstants.MaxStatNameLength)]
        public required string Name { get; set; }

        public int Base { get; set; }

        // TODO: The relationship should be m:n to reuse stats between different Pokemon. Now it is simplified
        public int PokemonId { get; set; }

        public Pokemon Pokemon { get; set; } = default!;
    }
}