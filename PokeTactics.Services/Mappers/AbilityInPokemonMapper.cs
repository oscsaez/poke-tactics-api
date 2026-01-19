using PokeTactics.Contracts.Ability.PokeApi;
using PokeTactics.Contracts.Ability.Responses;
using PokeTactics.Core.Entities;

namespace PokeTactics.Services.Mappers;

public static class AbilityInPokemonMapper
{
    // DTO -> Entity
    public static AbilityInPokemon ToAbilityInPokemon(this AbilitySlotPokeApiResponse abilitySlotPokeApiResponse)
    {
        return new AbilityInPokemon
        {
            Ability = abilitySlotPokeApiResponse.ToAbility(),
            IsHidden = abilitySlotPokeApiResponse.IsHidden
        };
    }

    public static ICollection<AbilityInPokemon> ToAbilitiesInPokemon(this ICollection<AbilitySlotPokeApiResponse> abilitySlotPokeApiResponses)
    {
        return [.. abilitySlotPokeApiResponses.Select(ToAbilityInPokemon)];
    }

    // Entity -> DTO
    public static AbilityDto ToAbilityDto(this AbilityInPokemon abilityInPokemon)
    {
        return new AbilityDto
        {
            Name = abilityInPokemon.Ability.Name,
            Description = abilityInPokemon.Ability.Description,
            IsHidden = abilityInPokemon.IsHidden
        };
    }

    public static ICollection<AbilityDto> ToAbilityDtos(this ICollection<AbilityInPokemon> abilitiesInPokemon)
    {
        return [.. abilitiesInPokemon.Select(ToAbilityDto)];
    }
}
