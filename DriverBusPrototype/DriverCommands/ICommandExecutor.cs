using DriverBusPrototype.DriverCommands.Models;

namespace DriverBusPrototype.DriverCommands;

public interface ICommandExecutor
{
    public CommandResult ExecuteCommand(Command command);

    Task<CommandResult> ExecuteCommandAsync(Command command);
}