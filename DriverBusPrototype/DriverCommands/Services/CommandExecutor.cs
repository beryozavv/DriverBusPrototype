using DriverBusPrototype.DriverCommands.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DriverBusPrototype.DriverCommands.Services;

internal class CommandExecutor : ICommandExecutor
{
    private readonly ICommunicationPort _communicationPort;
    private readonly TaskCompletionDictionaryProvider _dictionaryProvider;
    private readonly ILogger<CommandExecutor> _logger;
    private readonly DriverBusSettings _settings;

    public CommandExecutor(ICommunicationPort communicationPort, TaskCompletionDictionaryProvider dictionaryProvider,
        ILogger<CommandExecutor> logger, IOptions<DriverBusSettings> settings)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));

        _communicationPort = communicationPort;
        _dictionaryProvider = dictionaryProvider;
        _logger = logger;
        _settings = settings.Value;
    }

    public async Task<CommandResult> ExecuteCommandAsync(Command command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Write command to communication port. Command details: {Id} - {Type} - {Params}", command.Id, command.Type,
            command.Parameters);

        _communicationPort.Write(command);

        var taskCompletionSource = new TaskCompletionSource<CommandResult>(command.Id, TaskCreationOptions.None);

        using (cancellationToken.Register(() =>
                   taskCompletionSource.TrySetCanceled(cancellationToken)))
        {
            _dictionaryProvider.TaskCompletionSourcesDict.AddOrUpdate(command.Id, _ => taskCompletionSource,
                (_, _) => taskCompletionSource);

            return await taskCompletionSource.Task.WaitAsync(_settings.CommandTimeout, cancellationToken);
        }
    }
}