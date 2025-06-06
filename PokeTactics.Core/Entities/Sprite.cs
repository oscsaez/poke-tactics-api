using System.ComponentModel.DataAnnotations;
using PokeTactics.Core.Utils;

namespace PokeTactics.Core.Entities
{
    public class Sprite : Entity
    {
        [MaxLength(CoreConstants.MaxSpriteUriLength)]
        public required string OfficialArtworkUri { get; set; }

        [MaxLength(CoreConstants.MaxSpriteUriLength)]
        public string? BackMaleUri { get; set; }

        [MaxLength(CoreConstants.MaxSpriteUriLength)]
        public string? BackFemaleUri { get; set; }

        [MaxLength(CoreConstants.MaxSpriteUriLength)]
        public string? BackShinyMaleUri { get; set; }

        [MaxLength(CoreConstants.MaxSpriteUriLength)]
        public string? BackShinyFemaleUri { get; set; }

        [MaxLength(CoreConstants.MaxSpriteUriLength)]
        public string? FrontMaleUri { get; set; }

        [MaxLength(CoreConstants.MaxSpriteUriLength)]
        public string? FrontFemaleUri { get; set; }

        [MaxLength(CoreConstants.MaxSpriteUriLength)]
        public string? FrontShinyMaleUri { get; set; }

        [MaxLength(CoreConstants.MaxSpriteUriLength)]
        public string? FrontShinyFemaleUri { get; set; }
    }
}