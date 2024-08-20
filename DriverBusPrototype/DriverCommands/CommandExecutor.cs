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
            try
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

                if (command.Id != result.Id)
                {
                    throw new Exception("Command.Id and Result.Id mismatch");
                }

                return result;
            }
            finally
            {
                _communicationPort.Disconnect();
            }
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