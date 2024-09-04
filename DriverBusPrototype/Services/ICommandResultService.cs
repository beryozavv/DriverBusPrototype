namespace DriverBusPrototype.Services;

/// <summary>
/// Чтение и обработка результатов команд. Результаты получаем от драйвера
/// </summary>
public interface ICommandResultService
{
    Task ReadCommandsResults(CancellationToken stoppingToken);
}