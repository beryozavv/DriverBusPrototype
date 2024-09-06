namespace DriverBusPrototype.Models;

public class CommandResult
{
    public Guid Id { get; set; }
    public CommandType CommandType { get; set; }
    public bool IsSuccess { get; set; }
    public int? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Result { get; set; }
}