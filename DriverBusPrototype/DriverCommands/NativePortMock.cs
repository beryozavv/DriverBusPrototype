using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Text.Json;
using DriverBusPrototype.DriverCommands.Models;
using Xunit.Abstractions;

namespace DriverBusPrototype.DriverCommands;

internal class NativePortMock : INativePortMock
{
    private readonly TimeSpan _readTimeout;
    private readonly ITestOutputHelper _testOutputHelper;
    private string? _portName;
    private readonly ConcurrentStack<string> _commandIds = new();

    public NativePortMock(ITestOutputHelper testOutputHelper)
    {
        _readTimeout = Settings.ReadTimout;
        _testOutputHelper = testOutputHelper;
    }

    public bool Connect(string portName)
    {
        _portName = portName;
        _testOutputHelper.WriteLine("Connected to port " + portName);
        return true;
    }

    public IntPtr Read()
    {
        // драйвер готовит результат команды
        while (true)
        {
            Thread.Sleep(_readTimeout);
            if (_commandIds.TryPop(out var commandId))
            {
                var commandResult = GetTestResult(commandId);

                var dataPtr = StructureToPtrHelper.GetStructurePtr(commandResult);
                
                _testOutputHelper.WriteLine("Driver send result by commandId = " + commandId);

                return dataPtr;
            }
        }
    }

    public void Write(IntPtr commandPtr)
    {
        var command = Marshal.PtrToStructure<Command>(commandPtr);

        // драйвер обрабатывает команду:

        _commandIds.Push(command.Id);

        if (command.IsEncrypted)
        {
            throw new NotImplementedException();
        }

        if (command.Type == (int)CommandType.SetParams)
        {
            var paramsJson = JsonSerializer.Deserialize<ParamsJson>(command.Parameters);

            _testOutputHelper.WriteLine("command id = " + command.Id + "  file formats " +
                                        string.Join(',', paramsJson!.FileFormats));
        }
        else if (command.Type == (int)CommandType.SetPermissions)
        {
            var permissionsJson = JsonSerializer.Deserialize<PermissionsJson>(command.Parameters);

            _testOutputHelper.WriteLine("command id = " + command.Id + " userId = " + permissionsJson!.UserId);
        }
    }

    public void Disconnect()
    {
        _testOutputHelper.WriteLine("Disconnected from port " + _portName);
    }

    private CommandResult GetTestResult(string commandId)
    {
        return new CommandResult
        {
            Id = commandId,
            IsSuccess = false,
            ErrorCode = 123123,
            ErrorMessage =
                $"Error526526 Error123123 Error123123 Error123123 Error123123 Error123123 Error123123 Error123123 Error123123 commandId = {commandId}"
        };
    }
}