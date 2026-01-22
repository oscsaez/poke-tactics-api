using PokeTactics.Contracts.Ability.PokeApi;
using PokeTactics.Contracts.Move.PokeApi;
using PokeTactics.Contracts.Pokemon.PokeApi;
using PokeTactics.Contracts.Sprite.PokeApi;
using PokeTactics.Contracts.Stat.PokeApi;
using PokeTactics.Contracts.Type.PokeApi;

namespace PokeTactics.Api.Test.Utils.Builders;

public static class PokemonBuilder
{
    public static PokemonSummaryListPokeApiResponse BuildPokemonSummaryListPokeApiResponse(string name)
    {
        return new PokemonSummaryListPokeApiResponse
        {
            Count = 1,
            Results = [
                new PokemonSummaryPokeApiResponse
                {
                    Name = name,
                    Url = TestGenerator.RandomPath()
                }
            ]
        };
    }

    public static PokemonPokeApiResponse BuildPokemonPokeApiResponse(
        string name,
        ICollection<AbilitySummaryPokeApiResponse> abilitySummaries, 
        ICollection<MoveSummaryPokeApiResponse> moveSummaries)
    {
        PokemonPokeApiResponse pokemonResponse = new()
        {
            BaseExperience = TestGenerator.RandomInt(),
            Height = TestGenerator.RandomInt(),
            Name = name,
            Order = TestGenerator.RandomInt(),
            Sprite = new SpritePokeApiResponse
            {
                FrontMaleUri = TestGenerator.RandomGuidAsString(),
                OtherSprites = new OtherSpritesPokeApiResponse
                {
                    OfficialArtworkSprite = new OfficialArtworkSpritePokeApiResponse
                    {
                        FrontMaleUri = TestGenerator.RandomGuidAsString()
                    }
                }
            },
            Stats = [
                new StatPokeApiResponse
                {
                    Base = TestGenerator.RandomInt(),
                    Stat = new StatInfoPokeApiResponse
                    {
                        Name = TestGenerator.RandomName()
                    }
                }
            ],
            Types = [
                new TypePokeApiResponse
                {
                    TypeInfoPokeApiResponse = new TypeInfoPokeApiResponse
                    {
                        Name = TestGenerator.RandomName()
                    }
                }
            ],
            Weight = TestGenerator.RandomInt(),
            Abilities = [],
            Moves = []
        };

        foreach (var abilitySummary in abilitySummaries)
        {
            pokemonResponse.Abilities.Add(new AbilitySlotPokeApiResponse
            {
                IsHidden = false,
                Slot = 0,
                AbilityInfo = abilitySummary
            });
        }

        foreach (var moveSummary in moveSummaries)
        {
            pokemonResponse.Moves.Add(new MovePokeApiResponse
            {
                MoveUriPokeApiResponse = new MoveUriPokeApiResponse
                {
                    Name = moveSummary.Name,
                    Url = moveSummary.Url
                }
            });
        }

        return pokemonResponse;
    }
}
