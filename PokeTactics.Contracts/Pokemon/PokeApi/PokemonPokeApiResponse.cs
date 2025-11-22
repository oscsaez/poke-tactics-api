using System.Text.Json.Serialization;
using PokeTactics.Contracts.Ability.PokeApi;
using PokeTactics.Contracts.Move.PokeApi;
using PokeTactics.Contracts.Sprite.PokeApi;
using PokeTactics.Contracts.Stat.PokeApi;
using PokeTactics.Contracts.Type.PokeApi;

namespace PokeTactics.Contracts.Pokemon.PokeApi
{
    public class PokemonPokeApiResponse
    {
        public int? Order { get; set; }

        public required string Name { get; set; }

        public double Height { get; set; }

        public double Weight { get; set; }

        [JsonPropertyName("base_experience")]
        public int? BaseExperience { get; set; }

        public required ICollection<TypePokeApiResponse> Types { get; set; }

        public required ICollection<StatPokeApiResponse> Stats { get; set; }

        public required ICollection<AbilitySlotPokeApiResponse> Abilities { get; set; }

        public required ICollection<MovePokeApiResponse> Moves { get; set; }

        [JsonPropertyName("sprites")]
        public required SpritePokeApiResponse Sprite { get; set; }
    }
}