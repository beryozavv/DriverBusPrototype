namespace DriverBusPrototype.DriverCommands;

public interface ICommunicationPort // todo IDisposable?
{
    /// <summary>
    /// Подключение к порту
    /// </summary>
    /// <param name="portName">имя порта</param>
    /// <returns>Валиден ли порт</returns>
    bool Connect(string portName);

    /// <summary>
    /// Отключение от текущего порта
    /// </summary>
    void Disconnect();

    /// <summary>
    /// Считывание данных из текущего порта
    /// </summary>
    /// <returns>Результат выполнения команды</returns>
    T Read<T>() where T:struct;

    /// <summary>
    /// Запись данных в текущий порт
    /// </summary>
    /// <param name="command">Команда на выполнение</param>
    void Write<T>(T command) where T : struct;
}