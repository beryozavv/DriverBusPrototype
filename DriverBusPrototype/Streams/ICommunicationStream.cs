namespace DriverBusPrototype.Streams;

/// <summary>
/// Базовый интерфейс для коммуникации с драйвером
/// </summary>
public interface ICommunicationStream : IDisposable
{
    /// <summary>
    /// Подключение
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Валиден ли порт</returns>
    Task ConnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Считывание данных
    /// </summary>
    /// <returns>Результат выполнения команды</returns>
    Task<T> ReadAsync<T>() where T: class;

    /// <summary>
    /// Запись данных
    /// </summary>
    /// <param name="command">Команда на выполнение</param>
    Task WriteAsync<T>(T command) where T : class;
}