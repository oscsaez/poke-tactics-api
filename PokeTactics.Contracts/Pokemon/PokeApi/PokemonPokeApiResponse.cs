using PokeTactics.Contracts.Ability.PokeApi;

namespace PokeTactics.Contracts.Pokemon.PokeApi
{
    public class PokemonPokeApiResponse
    {
        public int Order { get; set; }

        public required string Name { get; set; }

        public double Height { get; set; }

        public double weight { get; set; }

        // TODO Rest of the properties

        public required ICollection<AbilitySlotPokeApiResponse> Abilities { get; set; }

        public int BaseExperience { get; set; }


    }
}