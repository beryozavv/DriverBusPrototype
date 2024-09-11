using DriverBusPrototype.Models;
using DriverBusPrototype.Services;
using DriverBusPrototype.Streams;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DriverBusPrototype.Tests.Mocks;

internal class CommandExecutorMock : CommandExecutor
{
    private readonly IOutputCommunicationStream _stream;
    private readonly ILogger<CommandExecutorMock> _logger;

    private static readonly TaskCompletionDictionaryProvider DictionaryProvider = new();

    public CommandExecutorMock(IOutputCommunicationStream stream, ILogger<CommandExecutorMock> logger,
        IOptions<DriverBusSettings> settings) : base(stream, DictionaryProvider, logger, settings)
    {
        _stream = stream;
        _logger = logger;
        
        _ = Task.Factory.StartNew(ReadResults,
            default, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();
    }

    private async Task ReadResults()
    {
        while (true)
        {
            try
            {
                var commandResult = await _stream.ReadAsync<CommandResult>();

                if (DictionaryProvider.TaskCompletionSourcesDict.TryGetValue(commandResult.Id,
                        out var taskCompletionSource))
                {
                    if (taskCompletionSource.TrySetResult(commandResult))
                    {
                        _logger.LogInformation("For command {Id} was set command result to task", commandResult.Id);
                    }

                    DictionaryProvider.TaskCompletionSourcesDict.TryRemove(commandResult.Id, out _);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "ReadResults error");
            }
        }
        // ReSharper disable once FunctionNeverReturns
    }
}