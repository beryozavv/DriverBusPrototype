namespace DriverBusPrototype;

public class CommunicationStreamException : Exception
{
    public CommunicationStreamException(string? message) : base(message)
    {
    }

    public CommunicationStreamException(string? message, Exception? innerException = null) : base(message, innerException)
    {
    
    }
}