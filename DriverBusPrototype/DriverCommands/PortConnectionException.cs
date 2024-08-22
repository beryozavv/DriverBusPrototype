namespace DriverBusPrototype.DriverCommands;

public class PortConnectionException : Exception
{
    public string? PortName { get; }

    public PortConnectionException(string? message) : base(message)
    {
    }

    public PortConnectionException(string? message, string? portName, Exception? innerException = null) : base(message, innerException)
    {
        PortName = portName;
    }
}