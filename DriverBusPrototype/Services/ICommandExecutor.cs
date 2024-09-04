using DriverBusPrototype.Models;

namespace DriverBusPrototype.Services;

/// <summary>
/// Отправка команд в драйвер и ожидание ответа
/// </summary>
public interface ICommandExecutor
{
    /// <summary>
    /// Отправить команду в драйвер и (асинхронно) получить ответ
    /// </summary>
    /// <param name="command">Команда</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Результат от драйвера</returns>
    Task<CommandResult> ExecuteCommandAsync(Command command, CancellationToken cancellationToken = default);
}