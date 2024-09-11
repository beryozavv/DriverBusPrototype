using DriverBusPrototype.Models;
using DriverBusPrototype.Streams;
using Microsoft.Extensions.Logging;

namespace DriverBusPrototype.Services;

internal class CommandResultService : ICommandResultService
{
    private readonly ILogger<CommandResultService> _logger;
    private readonly IOutputCommunicationStream _outputStream;
    private readonly TaskCompletionDictionaryProvider _dictionaryProvider;

    public CommandResultService(ILogger<CommandResultService> logger, IOutputCommunicationStream outputStream,
        TaskCompletionDictionaryProvider dictionaryProvider)
    {
        _logger = logger;
        _outputStream = outputStream;
        _dictionaryProvider = dictionaryProvider;
    }

    public async Task ReadCommandsResults(CancellationToken stoppingToken)
    {
        await _outputStream.ConnectAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await ReadCommandResult();
        }

        stoppingToken.ThrowIfCancellationRequested();
    }

    private async Task ReadCommandResult()
    {
        try
        {
            var commandResult = await _outputStream.ReadAsync<CommandResult>();

            if (_dictionaryProvider.TaskCompletionSourcesDict.TryGetValue(commandResult.Id,
                    out var taskCompletionSource))
            {
                if (taskCompletionSource.TrySetResult(commandResult))
                {
                    _logger.LogInformation("For command {Id} was set command result to task", commandResult.Id);
                }

                _dictionaryProvider.TaskCompletionSourcesDict.TryRemove(commandResult.Id, out _);
            }
            else
            {
                _logger.LogError("Command result not found in TaskCompletionSourcesDict. Command id = {Id}",
                    commandResult.Id);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "readCommandError");
        }
    }
}