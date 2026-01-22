using PokeTactics.Api.Test.Contexts;
using PokeTactics.Api.Test.Fixture;
using PokeTactics.Contracts.Ability.PokeApi;
using PokeTactics.Contracts.Move.PokeApi;
using PokeTactics.Contracts.Pokemon.PokeApi;
using PokeTactics.Contracts.Pokemon.Responses;
using PokeTactics.Core.Entities;
using PokeTactics.Core.Interfaces;

namespace PokeTactics.Api.Test.Utils.Helpers;

public static class PokemonVerifier
{
    public static async Task VerifyAbilitiesMovesAndPokemon(this PokeTacticsFixture fixture, SyncTestContext context)
    {
        Dictionary<string, AbilityEffectPokeApiResponse> abilityEffectByNameMap = new()
        {
            { context.AbilitySummaryResponse!.Name, context.AbilityEffectResponse! }
        };

        Dictionary<string, MoveInfoPokeApiResponse> movesInfoByNameMap = new()
        {
            { context.MoveSummaryResponse!.Name, context.MoveInfoResponse! }
        };

        await fixture.VerifyAbilities(abilityEffectByNameMap);
        await fixture.VerifyMoves(movesInfoByNameMap);
        await fixture.VerifyPokemon([context.PokemonResponse!]);
    }

    public static async Task VerifyPokemonDto(this PokeTacticsFixture fixture, PokemonDto response)
    {
        Pokemon expectedPokemon = await fixture.GetPokemon();

        VerifySimplePokemonDto(expectedPokemon, response);
        SpriteVerifier.VerifySpriteDto(expectedPokemon.Sprite, response.Sprite);
        AbilityVerifier.VerifyAbilityDtos(expectedPokemon.AbilitiesInPokemon, response.Abilities);
        MoveVerifier.VerifyMoveDtos([.. expectedPokemon.MovesInPokemon.Select(x => x.Move)], response.Moves);
    }

    public static async Task VerifySimplePokemonDto(this PokeTacticsFixture fixture, PokemonDto response)
    {
        Pokemon expectedPokemon = await fixture.GetPokemon();
        VerifySimplePokemonDto(expectedPokemon, response);
    }

    private static async Task VerifyPokemon(this PokeTacticsFixture fixture, IEnumerable<PokemonPokeApiResponse> pokemonResponseList)
    {
        IUnitOfWork unitOfWork = fixture.GetService<IUnitOfWork>();
        IEnumerable<Pokemon> allPokemons = await unitOfWork.PokemonDao.LoadAllAsync();

        foreach (var pokemonResponse in pokemonResponseList)
        {
            Pokemon pokemon = Assert.Single(allPokemons, x => x.Name == pokemonResponse.Name);
            Assert.Equal(pokemonResponse.Weight, pokemon.Weight);
            Assert.Equal(pokemonResponse.Height, pokemon.Height);
            Assert.Equal(pokemonResponse.Order, pokemon.PokedexOrder);
            Assert.Equal(pokemonResponse.Types.Select(x => x.TypeInfoPokeApiResponse.Name), pokemon.Types);
            Assert.Equal(pokemonResponse.Stats.ToDictionary(x => x.Stat.Name, x => x.Base), pokemon.Stats.ToDictionary(x => x.Name, x => x.Base));
            SpriteVerifier.VerifySprite(pokemonResponse.Sprite, pokemon.Sprite);
            VerifyAbilitiesInPokemon(pokemonResponse.Abilities, pokemon.AbilitiesInPokemon);
            VerifyMovesInPokemon(pokemonResponse.Moves, pokemon.MovesInPokemon);
        }
    }

    private static void VerifyAbilitiesInPokemon(
        IEnumerable<AbilitySlotPokeApiResponse> abilitySlotsResponse, 
        IEnumerable<AbilityInPokemon> abilitiesInPokemon)
    {
        foreach (var abilitySlotResponse in abilitySlotsResponse)
        {
            AbilityInPokemon abilityInPokemon = Assert.Single(abilitiesInPokemon, x => x.Ability.Name == abilitySlotResponse.AbilityInfo.Name);
            Assert.Equal(abilitySlotResponse.IsHidden, abilityInPokemon.IsHidden);
        }
    }

    private static void VerifyMovesInPokemon(IEnumerable<MovePokeApiResponse> movesResponse, IEnumerable<MoveInPokemon> movesInPokemon)
    {
        foreach (var moveResponse in movesResponse)
        {
            Assert.Single(movesInPokemon, x => x.Move.Name == moveResponse.MoveUriPokeApiResponse.Name);
        }
    }

    private static void VerifySimplePokemonDto(Pokemon expectedPokemon, PokemonDto actualPokemon)
    {
        Assert.Equal(expectedPokemon.Name, actualPokemon.Name);
        Assert.Equal(expectedPokemon.Weight, actualPokemon.Weight);
        Assert.Equal(expectedPokemon.Height, actualPokemon.Height);
        Assert.Equal(expectedPokemon.PokedexOrder, actualPokemon.PokedexOrder);
        Assert.Equal(expectedPokemon.Types, actualPokemon.Types);
        Assert.Equal(expectedPokemon.Stats.ToDictionary(x => x.Name, x => x.Base), actualPokemon.Stats.ToDictionary(x => x.Name, x => x.Base));
    }

    private static async Task<Pokemon> GetPokemon(this PokeTacticsFixture fixture)
    {
        IUnitOfWork unitOfWork = fixture.GetService<IUnitOfWork>();
        IEnumerable<Pokemon> allPokemons = await unitOfWork.PokemonDao.LoadAllAsync();
        
        return Assert.Single(allPokemons);
    }
}
