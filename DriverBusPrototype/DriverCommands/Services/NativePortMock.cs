using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Text.Json;
using DriverBusPrototype.DriverCommands.Helpers;
using DriverBusPrototype.DriverCommands.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DriverBusPrototype.DriverCommands.Services;

internal class NativePortMock : INativePort
{
    private readonly ILogger<NativePortMock> _logger;
    private readonly DriverBusSettings _settings;
    private string? _portName;
    private readonly ConcurrentStack<string> _commandIds = new();
    private int _readCounter;
    private int _writeCounter;

    public NativePortMock(ILogger<NativePortMock> logger, IOptions<DriverBusSettings> settings)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));

        _logger = logger;
        _settings = settings.Value;
    }

    public bool Connect(string portName)
    {
        _portName = portName;
        _logger.LogInformation("Connected to port {Name}", portName);
        return true;
    }

    public void Read(out IntPtr dataPtr, out int dataSize)
    {
        // драйвер готовит результат команды
        while (true)
        {
            Thread.Sleep(_settings.ReadTimeout);
            Interlocked.Increment(ref _readCounter);
            if (_settings.NativePortMockExceptions && _readCounter % 2 == 0)
            {
                throw new PortConnectionException("test ex");
            }

            if (_commandIds.TryPop(out var commandId))
            {
                var commandResult = GetTestResult(commandId);

                (dataPtr, dataSize) = ProtoConverter.ObjectToProtoPtr(commandResult);

                _logger.LogInformation("Driver send result by commandId = {Id}", commandId);

                return;
            }
        }
    }

    public void Write(IntPtr commandPtr, int size)
    {
        Interlocked.Increment(ref _writeCounter);
        if (_settings.NativePortMockExceptions && _writeCounter % 2 == 1)
        {
            throw new PortConnectionException("test ex");
        }

        var command = ProtoConverter.ObjectFromProtoPtr<Command>(commandPtr, size);

        Marshal.FreeHGlobal(commandPtr);

        // драйвер обрабатывает команду:

        _commandIds.Push(command.Id);

        if (command.IsEncrypted)
        {
            _logger.LogWarning("Command is encrypted");
        }

        if (command.Type == CommandType.SetParams)
        {
            var paramsJson = JsonSerializer.Deserialize<ParamsJson>(command.Parameters);

            _logger.LogInformation("command id = {Id} file formats = {Formats}", command.Id,
                string.Join(',', paramsJson!.FileFormats!));
        }
        else if (command.Type == CommandType.SetPermissions)
        {
            var permissionsJson = JsonSerializer.Deserialize<PermissionsJson>(command.Parameters);

            _logger.LogInformation("command id = {Id} userId = {UserId}", command.Id, permissionsJson!.UserId);
        }
    }

    public void Disconnect()
    {
        _logger.LogInformation("Disconnected from port {Port}", _portName);
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