using System.Runtime.InteropServices;
using System.Text.Json;
using DriverBusPrototype.DriverCommands;
using DriverBusPrototype.DriverCommands.Models;
using Xunit.Abstractions;

namespace DriverBusPrototype.Tests;

public class SocketMock : ISocket
{
    private readonly ITestOutputHelper _testOutputHelper;
    private string _portName;
    private string _commandId = "default";

    public SocketMock(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    public bool Connect(string portName)
    {
        _portName = portName;
        _testOutputHelper.WriteLine("Connected to port " + portName);
        return true;
    }

    public void Read(IntPtr dataPtr, int dataSize)
    {
        var commandResult = GetTestResult();

        Marshal.StructureToPtr(commandResult, dataPtr, false);

        _testOutputHelper.WriteLine("Send result by commandId = " + commandResult.Id);
    }

    public void Write(IntPtr dataPtr, int dataSize)
    {
        var command = Marshal.PtrToStructure<Command>(dataPtr);
        _commandId = command.Id;

        if (command.IsEncrypted)
        {
            throw new NotImplementedException();
        }

        if (command.Type == (int) CommandType.SetParams)
        {
            var paramsJson = JsonSerializer.Deserialize<ParamsJson>(command.Parameters);

            _testOutputHelper.WriteLine("command id = " + command.Id + "file formats " + string.Join(',', paramsJson!.FileFormats));
        }
        else
        {
            //todo
        }
    }

    public void Disconnect()
    {
        _testOutputHelper.WriteLine("Disconnected from port " + _portName);
    }

    private CommandResult GetTestResult()
    {
        return new CommandResult
        {
            Id = _commandId,
            IsSuccess = false,
            ErrorCode = 123123,
            ErrorMessage = "Error526526 Error123123 Error123123 Error123123 Error123123 Error123123 Error123123 Error123123 Error123123 qrwetqwetqwertqwertqrwtqwtwe"
        };
    }
}