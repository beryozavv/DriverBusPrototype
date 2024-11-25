using System.Text.Json;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace DriverBusPrototype.Streams;

internal abstract class BaseCommunicationStream : ICommunicationStream
{
    private readonly string _pipeName;
    private readonly AsyncRetryPolicy _retryPolicy;

    private ThreadSafetyOutGrpcStream _outputGrpcStream;
    
    private readonly SemaphoreSlim _reconnectSemaphore = new(1, 1);

    private const string CommandKey = "Command";
    private const string MethodKey = "Method";

    public BaseCommunicationStream(ILogger<BaseCommunicationStream> logger, string pipeName)
    {
        _pipeName = pipeName;

        _outputGrpcStream = new ThreadSafetyOutGrpcStream(_pipeName);

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
                        if (!_outputGrpcStream.IsConnected)
                        {
                            await ReconnectAsync(); //todo cancellation
                        }
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, "error reconnect in polly");
                    }
                });
    }

    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        await _outputGrpcStream.WaitForConnectionAsync(cancellationToken);
    }

    public async Task<T> ReadAsync<T>() where T : class
    {
        return await _retryPolicy.ExecuteAsync(async _ => // todo избавиться от замыканий?
            {
                try
                {
                    var readLine = await _outputGrpcStream.ReadLineAsync();
                    var commandResult = JsonSerializer.Deserialize<T>(readLine, BusJsonOptions.GetOptions());

                    if (commandResult == null)
                    {
                        throw new CommunicationStreamException("deserialize null");
                    }

                    return commandResult;
                }
                catch (ObjectDisposedException ex)
                {
                    throw new CommunicationStreamException("reader stream was disposed", ex);
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
                    await _outputGrpcStream.WriteLineAsync(commandJson);
                }
                catch (Exception ex) when (ex is IOException || ex is ObjectDisposedException)
                {
                    var message = ex is IOException ? "write IO exception" : "writer stream was disposed";
                    throw new CommunicationStreamException(message, ex);
                }
            },
            new Dictionary<string, object>
            {
                { CommandKey, command }, { MethodKey, nameof(WriteAsync) }
            });
    }

    public void Dispose()
    {
        _outputGrpcStream.Dispose();
    }

    private async Task ReconnectAsync(CancellationToken cancellationToken = default)
    {
        await _reconnectSemaphore.WaitAsync(cancellationToken);
        try
        {
            if (!_outputGrpcStream.IsConnected)
            {
                await _outputGrpcStream.DisposeAsync();
                
                ReplaceStreamInstance();
                
                await _outputGrpcStream.WaitForConnectionAsync(cancellationToken);
            }
        }
        finally
        {
            _reconnectSemaphore.Release();
        }
    }

    private void ReplaceStreamInstance()
    {
        var newStream = new ThreadSafetyOutGrpcStream(_pipeName);
        Interlocked.Exchange(ref _outputGrpcStream, newStream);
    }
}