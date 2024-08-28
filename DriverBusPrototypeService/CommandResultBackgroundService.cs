using DriverBusPrototype;
using DriverBusPrototype.DriverCommands;
using DriverBusPrototype.DriverCommands.Models;

namespace DriverBusPrototypeService;

public class CommandResultBackgroundService : BackgroundService
{
    private readonly ILogger<CommandResultBackgroundService> _logger;
    private readonly ICommunicationPort _communicationPort;
    private readonly TaskCompletionDictionaryProvider _dictionaryProvider;

    public CommandResultBackgroundService(ILogger<CommandResultBackgroundService> logger,
        ICommunicationPort communicationPort, TaskCompletionDictionaryProvider dictionaryProvider)
    {
        _logger = logger;
        _communicationPort = communicationPort;
        _dictionaryProvider = dictionaryProvider;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        Settings.NativeCommandMockExceptions = false; //todo
        _communicationPort.Connect("test port todo from config"); // todo
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _communicationPort.Dispose(); // todo singleton тоже диспозится в DI
        return base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service running. Проверка кириллицы");

        try
        {
            var readResultsTask = Task.Run(() =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    ReadResults(); // todo зачем бесконечно читать результаты, если у нас всего пару команд?
                }

                stoppingToken.ThrowIfCancellationRequested();
            }, stoppingToken);
            await readResultsTask;
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");
        }
    }

    private void ReadResults()
    {
        try
        {
            var commandResult = _communicationPort.Read<CommandResult>();

            if (_dictionaryProvider.TaskCompletionSourcesDict.TryGetValue(commandResult.Id, out var taskCompletionSource))
            {
                if (taskCompletionSource.TrySetResult(commandResult))
                {
                    _logger.LogInformation("For command {id} was set command result to task",commandResult.Id);
                }

                _dictionaryProvider.TaskCompletionSourcesDict.TryRemove(commandResult.Id, out _);
            }
            else
            {
                _logger.LogError("Read command not found. Command id = {Id}", commandResult.Id);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "readCommandError");
        }
    }
}