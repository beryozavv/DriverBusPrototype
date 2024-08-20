using DriverBusPrototype.DriverCommands.Models;

namespace DriverBusPrototype.DriverCommands;

public interface ICommandExecutor
{
    public CommandResult SendCommand(Command command);
}