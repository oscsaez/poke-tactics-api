using PokeTactics.Api.Test.Fixture;

namespace PokeTactics.Api.Test.HostedServices;

[Collection(PokeTacticsCollection.Name)]
public class PokemonSyncHostedServiceTest
{
    private readonly PokeTacticsFixture _fixture;

    public PokemonSyncHostedServiceTest(PokeTacticsFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Sync_NewAbilitiesMovesAndPokemon_CreateThem()
    {
        
    }
}
