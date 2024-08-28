namespace DriverBusPrototype.DriverCommands;

public class DriverBusSettings
{
    public string CommunicationPort { get; init; } = null!;
    public TimeSpan CommandTimeout { get; init; }
    public TimeSpan ReadTimeout { get; init; }
    public bool NativePortMockExceptions { get; init; }
}