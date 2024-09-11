using DriverBusPrototype.Models;
using DriverBusPrototype.Streams;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DriverBusPrototype.Services;

internal class CommandExecutor : ICommandExecutor
{
    private readonly IOutputCommunicationStream _communicationStream;
    private readonly TaskCompletionDictionaryProvider _dictionaryProvider;
    private readonly ILogger<CommandExecutor> _logger;
    private readonly DriverBusSettings _settings;

    public CommandExecutor(IOutputCommunicationStream communicationStream, TaskCompletionDictionaryProvider dictionaryProvider,
        ILogger<CommandExecutor> logger, IOptions<DriverBusSettings> settings)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));

        _communicationStream = communicationStream;
        _dictionaryProvider = dictionaryProvider;
        _logger = logger;
        _settings = settings.Value;
    }

    public async Task<CommandResult> ExecuteCommandAsync(Command command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "WriteAsync command to communication port. Command details: {Id} - {Type} - {Params}", command.Id, command.Type,
            command.Parameters);

        var taskCompletionSource = new TaskCompletionSource<CommandResult>(command.Id, TaskCreationOptions.None);

        using (cancellationToken.Register(() =>
                   taskCompletionSource.TrySetCanceled(cancellationToken)))
        {
            try
            {
                _dictionaryProvider.TaskCompletionSourcesDict.AddOrUpdate(command.Id, _ => taskCompletionSource,
                    (_, _) => taskCompletionSource);
            
                await _communicationStream.WriteAsync(command);
            }
            catch (Exception e)
            {
                _dictionaryProvider.TaskCompletionSourcesDict.TryRemove(command.Id, out var removedTaskCompletionSource);
                removedTaskCompletionSource?.TrySetException(e);
            }
            
            return await taskCompletionSource.Task.WaitAsync(_settings.CommandTimeout, cancellationToken);
        }
    }
}