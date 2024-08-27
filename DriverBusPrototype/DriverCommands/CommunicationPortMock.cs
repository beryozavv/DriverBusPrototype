using System.Runtime.InteropServices;
using DriverBusPrototype.DriverCommands.Models;
using Polly;
using Polly.Retry;
using Xunit.Abstractions;

namespace DriverBusPrototype.DriverCommands;

internal class CommunicationPortMock : ICommunicationPort
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly INativePortMock _nativePortMock;
    private readonly RetryPolicy _retryPolicy;
    private string? _portName;

    private const string NativePortKey = "NativePort";
    private const string LoggerKey = "Logger";
    private const string PortKey = "Port";
    private const string CommandKey = "Command";
    private const string MethodKey = "Method";

    public CommunicationPortMock(ITestOutputHelper testOutputHelper, INativePortMock nativePortMock)
    {
        _testOutputHelper = testOutputHelper;
        _nativePortMock = nativePortMock;

        _retryPolicy = Policy
            .Handle<PortConnectionException>()
            .WaitAndRetry(3, retryAttempt =>
                    TimeSpan.FromMilliseconds(200 * Math.Pow(2, retryAttempt)),
                static (exception, timeSpan, retryCount, context) =>
                {
                    // Log the exception or perform any other action on failure
                    ((ITestOutputHelper)context[LoggerKey]).WriteLine(
                        $"{DateTime.UtcNow:O} Method{context[MethodKey]} Retry {retryCount} encountered an error: {exception.Message}. Waiting {timeSpan} before next retry.");
                    if (context.TryGetValue(NativePortKey, out var nativePort))
                    {
                        var portMock = nativePort as INativePortMock;
                        portMock!.Connect(context[PortKey].ToString()!); // todo как проверить что соединение установлено, чтобы не устанавливать повторно?
                    }
                });
    }

    public bool Connect(string portName)
    {
        _portName = portName;
        return _nativePortMock.Connect(portName);
    }

    public T? Read<T>() where T : class
    {
        return _retryPolicy.Execute(static context =>
            {
                ((ITestOutputHelper)context[LoggerKey]).WriteLine("Начинаем считывание результатов команд из драйвера");
                ((INativePortMock)context[NativePortKey]).Read(out var resultPtr, out var size);

                var commandResult = ProtoConverter.ObjectFromProtoPtr<T>(resultPtr, size);
                
                Marshal.FreeHGlobal(resultPtr);

                return commandResult;
            },
            new Dictionary<string, object>
                { { NativePortKey, _nativePortMock }, { LoggerKey, _testOutputHelper }, { PortKey, _portName! }, {MethodKey, nameof(Read)} });
    }

    public void Write<T>(T command) where T : class
    {
        _retryPolicy.Execute(static context =>
            {
                var (commandPtr, size) = ProtoConverter.ObjectToProtoPtr((T)context[CommandKey]);

                ((ITestOutputHelper)context[LoggerKey]).WriteLine(
                    $"Отправляем в драйвер указатель на команду {((Command)context[CommandKey]).Id} commandPtr = " + commandPtr);

                ((INativePortMock)context[NativePortKey]).Write(commandPtr, size);
            },
            new Dictionary<string, object>
            {
                { NativePortKey, _nativePortMock }, { LoggerKey, _testOutputHelper }, { PortKey, _portName! },
                { CommandKey, command }, {MethodKey, nameof(Write)}
            });
    }

    public void Dispose()
    {
        _nativePortMock.Disconnect();
    }
}