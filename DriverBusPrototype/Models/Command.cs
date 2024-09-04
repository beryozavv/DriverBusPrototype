namespace DriverBusPrototype.Models;

public class Command
{
    public Guid Id { get; init; }
    public CommandType Type { get; init; }
    public bool IsEncrypted { get; init; }
    public string Parameters { get; set; } = null!; // todo init
}