namespace DriverBusPrototype.Services;

/// <summary>
/// Чтение и выполнение запросов от драйвера 
/// </summary>
public interface IRequestExecutor
{
    Task ExecuteRequests(CancellationToken stoppingToken);
}