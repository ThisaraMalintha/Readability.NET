namespace Readability.NET.Exceptions;

public class ReadabilityException : Exception
{
    public ReadabilityException()
    {
    }

    public ReadabilityException(string message) : base(message)
    {
    }

    public ReadabilityException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
