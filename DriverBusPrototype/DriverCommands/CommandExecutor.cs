using DriverBusPrototype.DriverCommands.Models;

namespace DriverBusPrototype.DriverCommands;

internal class CommandExecutor : ICommandExecutor
{
    private readonly ICommunicationPort _communicationPort;
    private readonly TaskCompletionDictionaryProvider _dictionaryProvider;

    public CommandExecutor(ICommunicationPort communicationPort, TaskCompletionDictionaryProvider dictionaryProvider)
    {
        _communicationPort = communicationPort;
        _dictionaryProvider = dictionaryProvider;
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
        _dictionaryProvider.TaskCompletionSourcesDict.AddOrUpdate(command.Id, _ => taskCompletionSource,
            (_, _) => taskCompletionSource);

        return await taskCompletionSource.Task.WaitAsync(Settings.CommandTimeout);
    }
}