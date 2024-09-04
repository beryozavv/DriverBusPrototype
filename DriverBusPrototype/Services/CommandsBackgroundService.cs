using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DriverBusPrototype.Services;

public class CommandsBackgroundService : BackgroundService
{
    private readonly ILogger<CommandsBackgroundService> _logger;
    private readonly ICommandResultService _commandResultService;
    private readonly IRequestExecutor _requestExecutor;

    public CommandsBackgroundService(ILogger<CommandsBackgroundService> logger,
        ICommandResultService commandResultService, IRequestExecutor requestExecutor)
    {
        _logger = logger;
        _commandResultService = commandResultService;
        _requestExecutor = requestExecutor;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service running");

        try
        {
            var readResultsTask = Task.Run(() =>
                _commandResultService.ReadCommandsResults(stoppingToken), stoppingToken);

            var readCommandsTask = Task.Run(() =>
                    _requestExecutor.ExecuteRequests(stoppingToken), stoppingToken);

            await Task.WhenAll(readResultsTask, readCommandsTask);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Timed Hosted Service is stopping");
        }
    }
}