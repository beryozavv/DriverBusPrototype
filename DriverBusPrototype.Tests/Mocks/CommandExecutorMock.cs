using DriverBusPrototype.DriverCommands;
using DriverBusPrototype.DriverCommands.Models;
using DriverBusPrototype.DriverCommands.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DriverBusPrototype.Tests.Mocks;

internal class CommandExecutorMock : CommandExecutor
{
    private readonly ICommunicationPort _communicationPort;
    private readonly ILogger<CommandExecutorMock> _logger;

    private static readonly TaskCompletionDictionaryProvider DictionaryProvider = new();

    public CommandExecutorMock(ICommunicationPort communicationPort, ILogger<CommandExecutorMock> logger,
        IOptions<DriverBusSettings> settings) : base(communicationPort, DictionaryProvider, logger, settings)
    {
        _communicationPort = communicationPort;
        _logger = logger;
        
        _ = Task.Run(ReadResults);
    }

    private void ReadResults()
    {
        while (true)
        {
            try
            {
                var commandResult = _communicationPort.Read<CommandResult>();

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