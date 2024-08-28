using System.Collections.Concurrent;
using DriverBusPrototype.DriverCommands.Models;
using Microsoft.Extensions.Logging;

namespace DriverBusPrototype.DriverCommands;

internal class CommandExecutorMock : ICommandExecutor
{
    private readonly ICommunicationPort _communicationPort;
    private readonly ILogger<CommandExecutorMock> _logger;

    private readonly ConcurrentDictionary<string, TaskCompletionSource<CommandResult>> _taskCompletionSourcesDict =
        new();

    public CommandExecutorMock(ICommunicationPort communicationPort, ILogger<CommandExecutorMock> logger)
    {
        _communicationPort = communicationPort;
        _logger = logger;

        //todo для теста. вынести в фоновый сервис
        _ = Task.Run(ReadResults);
    }

    private void ReadResults()
    {
        while (true)
        {
            try
            {
                var commandResult = _communicationPort.Read<CommandResult>();

                if (_taskCompletionSourcesDict.TryGetValue(commandResult.Id, out var taskCompletionSource))
                {
                    if (taskCompletionSource.TrySetResult(commandResult))
                    {
                        _logger.LogInformation($"For command {commandResult.Id} was set command result to task");
                    }

                    _taskCompletionSourcesDict.TryRemove(commandResult.Id, out _);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "ReadResults error");
            }
        }
        // ReSharper disable once FunctionNeverReturns
    }

    /// <summary>
    /// todo
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<CommandResult> ExecuteCommandAsync(Command command)
    {
        _communicationPort.Write(command);

        var taskCompletionSource = new TaskCompletionSource<CommandResult>(command.Id);
        _taskCompletionSourcesDict.AddOrUpdate(command.Id, _ => taskCompletionSource,
            (_, _) => taskCompletionSource);

        return await taskCompletionSource.Task.WaitAsync(Settings.CommandTimeout);
    }
}