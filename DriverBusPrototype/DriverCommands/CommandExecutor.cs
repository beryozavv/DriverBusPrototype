using System.Runtime.InteropServices;
using DriverBusPrototype.DriverCommands.Models;

namespace DriverBusPrototype.DriverCommands;

public class CommandExecutor : ICommandExecutor
{
    private readonly ISocket _socket;

    public CommandExecutor(ISocket socket)
    {
        _socket = socket;
    }

    public CommandResult SendCommand(Command command)
    {
        var isConnected = _socket.Connect("testPort");

        if (isConnected)
        {
            var (intPtr, size) = GetCommandPtr(command);
            _socket.Write(intPtr, size);

            // todo timeout

            var commandResult = GetTestResult(); // временно для выделения неуправляемой памяти
            var (resultPtr, resultSize) = GetCommandPtr(commandResult);

            _socket.Read(resultPtr, resultSize); // todo почему клиент задает?

            var result = Marshal.PtrToStructure<CommandResult>(resultPtr);

            _socket.Disconnect();

            return result;
        }
        else
        {
            throw new Exception("Invalid port name");
        }
    }

    private (IntPtr, int) GetCommandPtr<T>(T command) where T : struct
    {
        var size = Marshal.SizeOf(command);
        var ptr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(command, ptr, false);

        return (ptr, size);
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