using System.Collections.Concurrent;
using DriverBusPrototype.DriverCommands;
using DriverBusPrototype.DriverCommands.Models;

namespace DriverBusPrototypeService;

public class CommandResultBackgroundService : BackgroundService
{
    private readonly ILogger<CommandResultBackgroundService> _logger;
    private readonly ICommunicationPort _communicationPort;

    public static ConcurrentDictionary<string, TaskCompletionSource<CommandResult>>
        TaskCompletionSourcesDict { get; } = // todo куда вынести?
        new();

    public CommandResultBackgroundService(ILogger<CommandResultBackgroundService> logger,
        ICommunicationPort communicationPort)
    {
        _logger = logger;
        _communicationPort = communicationPort;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");

        // When the timer should have no due-time, then do the work once now.
        DoWork();

        using PeriodicTimer timer = new(TimeSpan.FromMilliseconds(10));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                DoWork();
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");
        }
    }

    // Could also be a async method, that can be awaited in ExecuteAsync above
    private void DoWork()
    {
        try
        {
            var commandResult = _communicationPort.Read<CommandResult>();

            if (TaskCompletionSourcesDict.TryGetValue(commandResult.Id, out var taskCompletionSource))
            {
                if (taskCompletionSource.TrySetResult(commandResult))
                {
                    Console.WriteLine($"For command {commandResult.Id} was set command result to task");
                }

                TaskCompletionSourcesDict.TryRemove(commandResult.Id, out _);
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