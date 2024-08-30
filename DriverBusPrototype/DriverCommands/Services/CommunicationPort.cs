using System.Runtime.InteropServices;
using DriverBusPrototype.DriverCommands.Helpers;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace DriverBusPrototype.DriverCommands.Services;

internal class CommunicationPort : ICommunicationPort
{
    private readonly ILogger<CommunicationPort> _logger;
    private readonly INativePort _nativePort;
    private readonly RetryPolicy _retryPolicy;
    private string? _portName;

    private const string NativePortKey = "NativePort";
    private const string LoggerKey = "Logger";
    private const string PortKey = "Port";
    private const string CommandKey = "Command";
    private const string MethodKey = "Method";

    public CommunicationPort(ILogger<CommunicationPort> logger, INativePort nativePort)
    {
        _logger = logger;
        _nativePort = nativePort;

        _retryPolicy = Policy
            .Handle<PortConnectionException>()
            .WaitAndRetry(3, retryAttempt =>
                    TimeSpan.FromMilliseconds(200 * Math.Pow(2, retryAttempt)),
                static (exception, timeSpan, retryCount, context) =>
                {
                    // Log the exception or perform any other action on failure
                    ((ILogger<CommunicationPort>)context[LoggerKey]).LogInformation(
                        "{UtcNow} Method{Unknown} Retry {RetryCount} encountered an error: {ExceptionMessage}. Waiting {TimeSpan} before next retry",
                        DateTime.UtcNow, context[MethodKey], retryCount, exception.Message, timeSpan);
                    if (context.TryGetValue(NativePortKey, out var nativePort))
                    {
                        var portMock = nativePort as INativePort;
                        try
                        {
                            portMock!.Connect(context[PortKey]
                                    .ToString()
                                !); // todo как проверить что соединение установлено, чтобы не устанавливать повторно?
                        }
                        catch (Exception e)
                        {
                            ((ILogger<CommunicationPort>)context[LoggerKey]).LogError(e, "reconnect in polly");
                        }
                    }
                });
    }

    public bool Connect(string portName)
    {
        _portName = portName;
        return _nativePort.Connect(portName);
    }

    public T Read<T>() where T : class
    {
        return _retryPolicy.Execute(static context =>
            {
                ((ILogger<CommunicationPort>)context[LoggerKey]).LogInformation(
                    "Start Reading Command Results from driver");
                ((INativePort)context[NativePortKey]).Read(out var resultPtr, out var size);

                var commandResult = ProtoConverter.ObjectFromProtoPtr<T>(resultPtr, size);
                Marshal.FreeHGlobal(resultPtr);

                return commandResult;
            },
            new Dictionary<string, object>
            {
                { NativePortKey, _nativePort }, { LoggerKey, _logger }, { PortKey, _portName! },
                { MethodKey, nameof(Read) }
            });
    }

    public void Write<T>(T command) where T : class
    {
        _retryPolicy.Execute(static context =>
            {
                var (commandPtr, size) = ProtoConverter.ObjectToProtoPtr((T)context[CommandKey]);

                ((INativePort)context[NativePortKey]).Write(commandPtr, size);
            },
            new Dictionary<string, object>
            {
                { NativePortKey, _nativePort }, { LoggerKey, _logger }, { PortKey, _portName! },
                { CommandKey, command }, { MethodKey, nameof(Write) }
            });
    }

    public void Dispose()
    {
        _nativePort.Disconnect();
    }
}