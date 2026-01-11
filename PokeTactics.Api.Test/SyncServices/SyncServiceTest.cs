using Microsoft.Extensions.DependencyInjection;
using PokeTactics.Api.Test.Contexts;
using PokeTactics.Api.Test.Fixture;
using PokeTactics.Api.Test.Utils;
using PokeTactics.Contracts.Ability.PokeApi;
using PokeTactics.Contracts.EffectEntry.PokeApi;
using PokeTactics.Contracts.Language.PokeApi;
using PokeTactics.Contracts.Move.PokeApi;
using PokeTactics.Contracts.Pokemon.PokeApi;
using PokeTactics.Contracts.Sprite.PokeApi;
using PokeTactics.Contracts.Stat.PokeApi;
using PokeTactics.Contracts.Type.PokeApi;
using PokeTactics.Core.Entities;
using PokeTactics.Core.Interfaces;
using PokeTactics.Core.Interfaces.SyncServices;

namespace PokeTactics.Api.Test.SyncServices;

[Collection(PokeTacticsCollection.Name)]
public class SyncServiceTest : IAsyncLifetime
{
    private const string EnglishLanguage = "en";

    private readonly PokeTacticsFixture _fixture;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISyncService _syncService;

    public SyncServiceTest(PokeTacticsFixture fixture)
    {
        _fixture = fixture;
        _unitOfWork = _fixture.GetService<IUnitOfWork>();
        _syncService = _fixture.GetService<ISyncService>();
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _fixture.DeleteAll();
    }

    [Fact]
    public async Task Sync_NewAbilitiesMovesAndPokemon_CreateThem()
    {
        // Arrange
        CancellationToken cancellationToken = new();
        SyncTestContext context = SetupNewPokemon();

        // Act
        await _syncService.Sync(cancellationToken);

        // Assert
        await VerifyAbilitiesMovesAndPokemon(context);
    }

    [Fact]
    public async Task Sync_UpdateAbilitiesMovesAndPokemon_UpdateThem()
    {
        // Arrange
        CancellationToken cancellationToken = new();
        SyncTestContext context = SetupNewPokemon();

        await _syncService.Sync(cancellationToken);

        context.AbilityEffectResponse!.EffectEntries.Single().Effect = TestGenerator.RandomGuidAsString();
        _fixture.ConfigurePokeApiMockServerForGet(context.AbilitySummaryResponse!.Url, context.AbilityEffectResponse!);

        context.MoveInfoResponse!.EffectEntries.Single().Effect = TestGenerator.RandomGuidAsString();
        _fixture.ConfigurePokeApiMockServerForGet(context.MoveSummaryResponse!.Url, context.MoveInfoResponse!);

        context.PokemonResponse!.Order = TestGenerator.RandomInt();
        _fixture.ConfigurePokeApiMockServerForGet(context.PokemonSummaryResponse!.Url, context.PokemonResponse!);

        // Act
        await _syncService.Sync(cancellationToken);

        // Assert
        await VerifyAbilitiesMovesAndPokemon(context);
    }

    [Fact]
    public async Task Sync_RemoveAbilitiesMovesAndPokemonAndAddNewOnes_RemoveExistingOnesAndAddNewOnes()
    {
        // Arrange
        CancellationToken cancellationToken = new();
        SyncTestContext context1 = SetupNewPokemon();

        await _syncService.Sync(cancellationToken);
        await VerifyAbilitiesMovesAndPokemon(context1);

        SyncTestContext context2 = SetupNewPokemon();

        // Act
        await _syncService.Sync(cancellationToken);

        // Arrange
        await VerifyAbilitiesMovesAndPokemon(context2);
    }

#region Setup

    private SyncTestContext SetupNewPokemon()
    {
        string pokemonName = TestGenerator.RandomName();
        AbilitySummaryListPokeApiResponse abilitiesSummary = BuildAbilitySummaryListPokeApiResponse();
        AbilitySummaryPokeApiResponse abilitySummary = abilitiesSummary.Results.Single();
        AbilityEffectPokeApiResponse abilityEffect = BuildAbilityEffectPokeApiResponse();
        MoveSummaryListPokeApiResponse movesSummary = BuildMoveSummaryListPokeApiResponse();
        MoveSummaryPokeApiResponse moveSummary = movesSummary.Results.Single();
        MoveInfoPokeApiResponse moveInfo = BuildMoveInfoPokeApiResponse();
        PokemonSummaryListPokeApiResponse pokemonSummaryList = BuildPokemonSummaryListPokeApiResponse(pokemonName);
        PokemonSummaryPokeApiResponse pokemonSummary = pokemonSummaryList.Results.Single();
        PokemonPokeApiResponse pokemonResponse = BuildPokemonPokeApiResponse(pokemonName, [abilitySummary], [moveSummary]);

        _fixture.ConfigurePokeApiMockServerToGetAbilitiesSummary(abilitiesSummary);
        _fixture.ConfigurePokeApiMockServerForGet(abilitySummary.Url, abilityEffect);
        _fixture.ConfigurePokeApiMockServerToGetMovesSummary(movesSummary);
        _fixture.ConfigurePokeApiMockServerForGet(moveSummary.Url, moveInfo);
        _fixture.ConfigurePokeApiMockServerToGetPokemonSummaryList(pokemonSummaryList);
        _fixture.ConfigurePokeApiMockServerForGet(pokemonSummary.Url, pokemonResponse);

        return new SyncTestContext
        {
            AbilitySummaryResponse = abilitySummary,
            AbilityEffectResponse = abilityEffect,
            MoveSummaryResponse = moveSummary,
            MoveInfoResponse = moveInfo,
            PokemonSummaryResponse = pokemonSummary,
            PokemonResponse = pokemonResponse
        };
    }

#endregion

#region Builders

    private static AbilitySummaryListPokeApiResponse BuildAbilitySummaryListPokeApiResponse()
    {
        return new AbilitySummaryListPokeApiResponse
        {
            Count = 1,
            Results = [
                new AbilitySummaryPokeApiResponse
                {
                    Name = TestGenerator.RandomName(),
                    Url = TestGenerator.RandomPath()
                }
            ]
        };
    }

    private static AbilityEffectPokeApiResponse BuildAbilityEffectPokeApiResponse()
    {
        return new AbilityEffectPokeApiResponse
        {
            EffectEntries = [BuildRandomEffectEntry()]
        };
    }

    private static MoveSummaryListPokeApiResponse BuildMoveSummaryListPokeApiResponse()
    {
        return new MoveSummaryListPokeApiResponse
        {
            Count = 1,
            Results = [
                new MoveSummaryPokeApiResponse
                {
                    Name = TestGenerator.RandomName(),
                    Url = TestGenerator.RandomPath()
                }
            ]
        };
    }

    private static MoveInfoPokeApiResponse BuildMoveInfoPokeApiResponse()
    {
        return new MoveInfoPokeApiResponse
        {
            Accuracy = TestGenerator.RandomInt(),
            Power = TestGenerator.RandomInt(),
            Pp = TestGenerator.RandomInt(),
            Type = new TypeInfoPokeApiResponse
            {
                Name = TestGenerator.RandomName()
            },
            EffectEntries = [BuildRandomEffectEntry()]
        };
    }

    private static PokemonSummaryListPokeApiResponse BuildPokemonSummaryListPokeApiResponse(string name)
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

    private static PokemonPokeApiResponse BuildPokemonPokeApiResponse(
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

    private static EffectEntryPokeApiResponse BuildRandomEffectEntry()
    {
        return new EffectEntryPokeApiResponse
        {
            Effect = TestGenerator.RandomGuidAsString(),
            Language = new LanguagePokeApiResponse
            {
                Name = EnglishLanguage
            }
        };
    }

#endregion

#region Verifiers

    private async Task VerifyAbilitiesMovesAndPokemon(SyncTestContext context)
    {
        Dictionary<string, AbilityEffectPokeApiResponse> abilityEffectByNameMap = new()
        {
            { context.AbilitySummaryResponse!.Name, context.AbilityEffectResponse! }
        };

        Dictionary<string, MoveInfoPokeApiResponse> movesInfoByNameMap = new()
        {
            { context.MoveSummaryResponse!.Name, context.MoveInfoResponse! }
        };

        await VerifyAbilities(abilityEffectByNameMap);
        await VerifyMoves(movesInfoByNameMap);
        await VerifyPokemon([context.PokemonResponse!]);
    }

    private async Task VerifyAbilities(IDictionary<string, AbilityEffectPokeApiResponse> abilityEffectsByNameMap)
    {
        IEnumerable<Ability> allAbilities = await _unitOfWork.AbilityDao.LoadAllAsync();
        
        foreach (var abilityEffectByName in abilityEffectsByNameMap)
        {
            Ability ability = Assert.Single(allAbilities, x => x.Name == abilityEffectByName.Key);
            Assert.Equal(abilityEffectByName.Value.EffectEntries.Single().Effect, ability.Description);
        }
    }

    private async Task VerifyMoves(IDictionary<string, MoveInfoPokeApiResponse> moveInfosByNameMap)
    {
        IEnumerable<Move> allMoves = await _unitOfWork.MoveDao.LoadAllAsync();

        foreach (var moveInfoByName in moveInfosByNameMap)
        {
            Move move = Assert.Single(allMoves, x => x.Name == moveInfoByName.Key);
            Assert.Equal(moveInfoByName.Value.Accuracy, move.Accuracy);
            Assert.Equal(moveInfoByName.Value.Power, move.Power);
            Assert.Equal(moveInfoByName.Value.Pp, move.PowerPoints);
            Assert.Equal(moveInfoByName.Value.Type.Name, move.Type);
            Assert.Equal(moveInfoByName.Value.EffectEntries.Single().Effect, move.Description);
        }
    }

    private async Task VerifyPokemon(IEnumerable<PokemonPokeApiResponse> pokemonResponseList)
    {
        IEnumerable<Pokemon> allPokemons = await _unitOfWork.PokemonDao.LoadAllAsync();

        foreach (var pokemonResponse in pokemonResponseList)
        {
            Pokemon pokemon = Assert.Single(allPokemons, x => x.Name == pokemonResponse.Name);
            Assert.Equal(pokemonResponse.Weight, pokemon.Weight);
            Assert.Equal(pokemonResponse.Height, pokemon.Height);
            Assert.Equal(pokemonResponse.Order, pokemon.PokedexOrder);
            Assert.Equal(pokemonResponse.Types.Select(x => x.TypeInfoPokeApiResponse.Name), pokemon.Types);
            Assert.Equal(pokemonResponse.Stats.ToDictionary(x => x.Stat.Name, x => x.Base), pokemon.Stats.ToDictionary(x => x.Name, x => x.Base));
            VerifySprite(pokemonResponse.Sprite, pokemon.Sprite);
            VerifyAbilitiesInPokemon(pokemonResponse.Abilities, pokemon.AbilitiesInPokemon);
            VerifyMovesInPokemon(pokemonResponse.Moves, pokemon.MovesInPokemon);
        }
    }

    private static void VerifySprite(SpritePokeApiResponse expected, Sprite actual)
    {
        Assert.Equal(expected.BackFemaleUri, actual.BackFemaleUri);
        Assert.Equal(expected.BackMaleUri, actual.BackMaleUri);
        Assert.Equal(expected.BackShinyFemaleUri, actual.BackShinyFemaleUri);
        Assert.Equal(expected.BackShinyMaleUri, actual.BackShinyMaleUri);
        Assert.Equal(expected.FrontFemaleUri, actual.FrontFemaleUri);
        Assert.Equal(expected.FrontMaleUri, actual.FrontMaleUri);
        Assert.Equal(expected.FrontShinyFemaleUri, actual.FrontShinyFemaleUri);
        Assert.Equal(expected.FrontShinyMaleUri, actual.FrontShinyMaleUri);
        Assert.Equal(expected.OtherSprites.OfficialArtworkSprite.FrontMaleUri, actual.OfficialArtworkUri);
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

    #endregion

}
