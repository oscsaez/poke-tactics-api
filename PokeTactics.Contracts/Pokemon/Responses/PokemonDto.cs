using PokeTactics.Contracts.Ability.Responses;
using PokeTactics.Contracts.Move.Responses;
using PokeTactics.Contracts.Sprite.Responses;
using PokeTactics.Contracts.Stat.Responses;

namespace PokeTactics.Contracts.Pokemon.Responses;

// TODO: Use MaxLength? But it is a response. TBD. This TODO is for all responses
public class PokemonDto
{
    /// <summary>
    /// Order of the pokemon in the pokedex
    /// </summary>
    public int? PokedexOrder { get; set; }

    public required string Name { get; set; }

    public double Height { get; set; }

    public double Weight { get; set; }

    /// <summary>
    /// Types of the pokemon. There are two at most
    /// </summary>
    public IList<string> Types { get; set; } = [];

    /// <summary>
    /// Stats of the pokemon. They are six
    /// </summary>
    public ICollection<StatDto> Stats { get; set; } = [];

    public ICollection<AbilityDto> Abilities { get; set; } = [];

    public ICollection<MoveDto> Moves { get; set; } = [];

    /// <summary>
    /// It contains the URIs of the sprites (images) of the pokemon
    /// </summary>
    public required SpriteDto Sprite { get; set; }
}
