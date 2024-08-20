namespace DriverBusPrototype.DriverCommands;

public interface ISocket // todo IDisposable?
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
    /// <param name="dataPtr">Указатель на начало буфера</param>
    /// <param name="dataSize">Размер буфера</param>
    /// <returns>Сообщение</returns>
    void Read(IntPtr dataPtr, int dataSize); // todo нужен ли dataSize?

    /// <summary>
    /// Запись данных в текущий порт
    /// </summary>
    /// <param name="dataPtr">Указатель на начало буфера</param>
    /// <param name="dataSize">Размер буфера</param>
    void Write(IntPtr dataPtr, int dataSize);
}