using System.IO.Pipes;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace DriverBusPrototype.Services;

internal abstract class BaseNamedPipeStream : ICommunicationStream
{
    private readonly string _pipeName;
    private readonly AsyncRetryPolicy _retryPolicy;

    private NamedPipeServerStream _pipeServerStream;

    private const string CommandKey = "Command";
    private const string MethodKey = "Method";

    public BaseNamedPipeStream(ILogger<BaseNamedPipeStream> logger, string pipeName)
    {
        _pipeName = pipeName;

        _pipeServerStream = GetNewStream(_pipeName);

        _retryPolicy = Policy
            .Handle<CommunicationStreamException>()
            .WaitAndRetryAsync(3, retryAttempt =>
                    TimeSpan.FromMilliseconds(200 * Math.Pow(2, retryAttempt)),
                async (exception, timeSpan, retryCount, context) =>
                {
                    // Log the exception or perform any other action on failure
                    logger.LogInformation(
                        "{UtcNow} Method{Unknown} Retry {RetryCount} encountered an error: {ExceptionMessage}. Waiting {TimeSpan} before next retry",
                        DateTime.UtcNow, context[MethodKey], retryCount, exception.Message, timeSpan);
                    try
                    {
                        var readPipeConnect = Task.FromResult(_pipeServerStream);

                        //lock (Lock) // todo синхронизация одновременных реконнектов от разных команд

                        if (!_pipeServerStream.IsConnected)
                        {
                            readPipeConnect = ReconnectAsync(_pipeServerStream);
                        }
                        _pipeServerStream = await readPipeConnect;
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, "reconnect in polly");
                    }
                });
    }

    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        await _pipeServerStream.WaitForConnectionAsync(cancellationToken);
    }

    public async Task<T> ReadAsync<T>() where T : class
    {
        return await _retryPolicy.ExecuteAsync(async _ => // todo избавиться от замыканий?
            {
                using (var reader = new StreamReader(_pipeServerStream, leaveOpen: true))
                {
                    var readLine = await reader.ReadLineAsync();

                    if (string.IsNullOrEmpty(readLine))
                    {
                        readLine = await reader.ReadLineAsync(); //todo после второго чтения стает notConnected
                        if (string.IsNullOrEmpty(readLine))
                        {
                            throw new CommunicationStreamException("read null");
                        }
                    }

                    var commandResult = JsonSerializer.Deserialize<T>(readLine, BusJsonOptions.GetOptions());

                    if (commandResult == null)
                    {
                        throw new CommunicationStreamException("deserialize null");
                    }

                    return commandResult;
                }
            },
            new Dictionary<string, object>
            {
                { MethodKey, nameof(ReadAsync) }
            });
    }

    public async Task WriteAsync<T>(T command) where T : class
    {
        await _retryPolicy.ExecuteAsync(async _ =>
            {
                var commandJson = JsonSerializer.Serialize(command, BusJsonOptions.GetOptions());
                try
                {
                    using (var writer = new StreamWriter(_pipeServerStream, leaveOpen: true))
                    {
                        writer.AutoFlush = true;
                        await writer.WriteLineAsync(commandJson);
                    }
                }
                catch (Exception ex) when (ex is IOException || ex is ObjectDisposedException)
                {
                    throw new CommunicationStreamException("write IO exception", ex);
                }
            },
            new Dictionary<string, object>
            {
                { CommandKey, command }, { MethodKey, nameof(WriteAsync) }
            });
    }

    public void Dispose()
    {
        _pipeServerStream.Dispose();
    }

    private async Task<NamedPipeServerStream> ReconnectAsync(NamedPipeServerStream stream)
    {
        await stream.DisposeAsync();

        var newStream = GetNewStream(_pipeName);

        await newStream.WaitForConnectionAsync(); //todo cancellation

        return newStream;
    }

    private NamedPipeServerStream GetNewStream(string pipeName)
    {
        return new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1,
            PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
    }
}