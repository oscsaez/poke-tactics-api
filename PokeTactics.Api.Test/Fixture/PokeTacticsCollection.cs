namespace PokeTactics.Api.Test.Fixture;

[CollectionDefinition(Name)]
public class PokeTacticsCollection : ICollectionFixture<PokeTacticsFixture>
{
    public const string Name = "PokeTactics";
}
