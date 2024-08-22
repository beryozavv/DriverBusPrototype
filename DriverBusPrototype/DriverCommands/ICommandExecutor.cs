using DriverBusPrototype.DriverCommands.Models;

namespace DriverBusPrototype.DriverCommands;

public interface ICommandExecutor
{
    Task<CommandResult> ExecuteCommandAsync(Command command);
}