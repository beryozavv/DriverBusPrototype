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
        var commandResultReaderTask = Task.Run(() =>
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
    /// todo комменты
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public CommandResult ExecuteCommand(Command command)
    {
        // todo повторный коннект (несколько раз) при получении ошибки в операциях чтения/записи 
        var isConnected = _communicationPort.Connect("testPort"); // todo если порт отключен

        if (isConnected)
        {
            try
            {
                _communicationPort.Write(command);

                var result = OperationTimeoutHelper.OperationWithTimeout(
                    () => _communicationPort.Read<CommandResult>(), Settings.CommandTimeout);

                if (command.Id != result.Id)
                {
                    throw new Exception("Command.Id and Result.Id mismatch");
                }

                return result;
            }
            finally
            {
                _communicationPort.Disconnect(); // todo делаем 1 раз при завершении приложения
            }
        }
        else
        {
            throw new Exception("Invalid port name");
        }
    }

    /// <summary>
    /// todo
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<CommandResult> ExecuteCommandAsync(Command command)
    {
        // todo повторный коннект (несколько раз) при получении ошибки в операциях чтения/записи 
        var isConnected = _communicationPort.Connect("testPort"); // todo если порт отключен

        if (isConnected)
        {
            try
            {
                _communicationPort.Write(command);

                var taskCompletionSource = new TaskCompletionSource<CommandResult>(command.Id);
                _taskCompletionSourcesDict.AddOrUpdate(command.Id, _ => taskCompletionSource,
                    (_, _) => taskCompletionSource);

                return await taskCompletionSource.Task.WaitAsync(Settings.CommandTimeout);
            }
            finally
            {
                _communicationPort.Disconnect(); // todo делаем 1 раз при завершении приложения
            }
        }
        else
        {
            throw new Exception("Invalid port name");
        }
    }
}