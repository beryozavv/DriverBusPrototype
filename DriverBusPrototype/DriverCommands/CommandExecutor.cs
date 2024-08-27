using System.Collections.Concurrent;
using DriverBusPrototype.DriverCommands.Models;

namespace DriverBusPrototype.DriverCommands;

internal class CommandExecutor : ICommandExecutor
{
    private readonly ICommunicationPort _communicationPort;

    private readonly ConcurrentDictionary<string, TaskCompletionSource<CommandResult>> _taskCompletionSourcesDict =
        new();

    public CommandExecutor(ICommunicationPort communicationPort)
    {
        _communicationPort = communicationPort;

        //todo для теста. вынести в фоновый сервис
        var backgroundTask = Task.Run(() =>
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
                            Console.WriteLine($"For command {commandResult.Id} was set command result to task");
                        }

                        _taskCompletionSourcesDict.TryRemove(commandResult.Id, out _);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        });
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