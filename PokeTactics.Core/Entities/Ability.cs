using System.ComponentModel.DataAnnotations;
using PokeTactics.Core.Utils;

namespace PokeTactics.Core.Entities
{
    public class Ability : Entity
    {
        [MaxLength(CoreConstants.MaxAbilityNameLength)]
        public required string Name { get; set; }

        [MaxLength(CoreConstants.MaxDescriptionLength)]
        public string? Description { get; set; }

        public bool IsHidden { get; set; }

        public required ICollection<AbilitiesInPokemon> AbilitiesInPokemon { get; set; }
    }
}