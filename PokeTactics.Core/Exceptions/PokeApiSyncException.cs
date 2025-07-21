namespace PokeTactics.Core.Exceptions;

public class PokeApiSyncException : Exception
{
    public PokeApiSyncException()
    {
    }

    public PokeApiSyncException(string message) : base(message)
    {
    }

    public PokeApiSyncException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
