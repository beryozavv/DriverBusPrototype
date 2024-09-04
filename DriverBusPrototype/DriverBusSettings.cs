namespace DriverBusPrototype;

public class DriverBusSettings
{
    public string InputPipeName { get; init; } = null!;
    public string OutputPipeName { get; init; } = null!;
    public TimeSpan CommandTimeout { get; init; }
    public TimeSpan ReadTimeout { get; init; }
    public bool NativePortMockExceptions { get; init; }
}