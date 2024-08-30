using DriverBusPrototype.DriverCommands.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DriverBusPrototype.DriverCommands.Services;

public class CommandResultBackgroundService : BackgroundService
{
    private readonly ILogger<CommandResultBackgroundService> _logger;
    private readonly ICommunicationPort _communicationPort;
    private readonly TaskCompletionDictionaryProvider _dictionaryProvider;
    private readonly DriverBusSettings _settings;

    public CommandResultBackgroundService(ILogger<CommandResultBackgroundService> logger,
        ICommunicationPort communicationPort, TaskCompletionDictionaryProvider dictionaryProvider,
        IOptions<DriverBusSettings> settings)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        
        _logger = logger;
        _communicationPort = communicationPort;
        _dictionaryProvider = dictionaryProvider;
        _settings = settings.Value;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _communicationPort.Connect(_settings.CommunicationPort);
        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service running");

        try
        {
            var readResultsTask = Task.Run(() =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    ReadResults();
                }

                stoppingToken.ThrowIfCancellationRequested();
            }, stoppingToken);
            await readResultsTask;
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Timed Hosted Service is stopping");
        }
    }

    private void ReadResults()
    {
        try
        {
            var commandResult = _communicationPort.Read<CommandResult>();

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
                _logger.LogError("Read command result not found in TaskCompletionSourcesDict. Command id = {Id}",
                    commandResult.Id);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "readCommandError");
        }
    }
}