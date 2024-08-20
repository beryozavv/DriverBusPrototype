using System.Runtime.InteropServices;
using DriverBusPrototype.DriverCommands.Models;

namespace DriverBusPrototype.DriverCommands;

public class CommandExecutor : ICommandExecutor
{
    private readonly ICommunicationPort _communicationPort;

    public CommandExecutor(ICommunicationPort communicationPort)
    {
        _communicationPort = communicationPort;
    }

    public CommandResult SendCommand(Command command)
    {
        var isConnected = _communicationPort.Connect("testPort");

        if (isConnected)
        {
            var commandPtr = StructureToPtrHelper.GetCommandPtr(command);
            _communicationPort.Write(commandPtr, 0);

            var resultPtr = OperationTimeoutHelper.OperationWithTimeout(
                () =>
                {
                    _communicationPort.Read(out var resultPtr, out _);
                    return resultPtr;
                }
                , Settings.CommandTimeout);

            var result = Marshal.PtrToStructure<CommandResult>(resultPtr);

            _communicationPort.Disconnect();

            return result;
        }
        else
        {
            throw new Exception("Invalid port name");
        }
    }


    private CommandResult GetTestResult()
    {
        return new CommandResult
        {
            Id = Guid.NewGuid().ToString(),
            IsSuccess = false,
            ErrorCode = 123123,
            ErrorMessage = "Error123123 Error123123"
        };
    }
}