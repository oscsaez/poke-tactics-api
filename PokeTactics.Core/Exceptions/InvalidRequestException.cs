namespace PokeTactics.Core.Exceptions;

public class InvalidRequestException : Exception
{
    public InvalidRequestException() : base()
    {
    }

    public InvalidRequestException(string message) : base(message)
    {
    }

    public InvalidRequestException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
