using System.ComponentModel.DataAnnotations;
using PokeTactics.Core.Utils;

namespace PokeTactics.Core.Entities
{
    public class Stat : Entity
    {
        [MaxLength(CoreConstants.MaxStatNameLength)]
        public required string Name { get; set; }

        public int Base { get; set; }
    }
}