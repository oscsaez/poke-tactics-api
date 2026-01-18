using PokeTactics.Contracts.Ability.Responses;
using PokeTactics.Contracts.Move.Responses;
using PokeTactics.Contracts.Sprite.Responses;
using PokeTactics.Contracts.Stat.Responses;

namespace PokeTactics.Contracts.Pokemon.Responses;

public class PokemonDto
{
    public int? PokedexOrder { get; set; }

    public required string Name { get; set; }

    public double Height { get; set; }

    public double Weight { get; set; }

    public IList<string> Types { get; set; } = [];

    public ICollection<StatDto> Stats { get; set; } = [];

    public ICollection<AbilityDto> Abilities { get; set; } = [];

    public ICollection<MoveDto> Moves { get; set; } = [];

    public required SpriteDto Sprite { get; set; }
}
