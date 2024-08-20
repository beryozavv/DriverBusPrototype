namespace DriverBusPrototype.DriverCommands;

public class CommandTimeoutException : Exception
{
    public TimeSpan? Timeout { get; }

    public CommandTimeoutException(string? message) : base(message)
    {
    }

    public CommandTimeoutException(string? message, TimeSpan? timeout, Exception? innerException = null) : base(message, innerException)
    {
        Timeout = timeout;
    }
}