using System.ComponentModel.DataAnnotations;
using PokeTactics.Core.Utils;

namespace PokeTactics.Core.Entities
{
    public class Ability : Entity
    {
        [MaxLength(CoreConstants.MaxAbilityNameLength)]
        public required string Name { get; set; }

        public string? Description { get; set; }

        public bool IsHidden { get; set; }

        public ICollection<AbilitiesInPokemon> AbilitiesInPokemon { get; set; } = [];
    }
}