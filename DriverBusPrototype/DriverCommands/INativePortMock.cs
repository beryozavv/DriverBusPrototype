namespace DriverBusPrototype.DriverCommands;

public interface INativePortMock
{
    public bool Connect(string portName);
    public IntPtr Read(); // todo Здесь мы задаем буфер?   
    public void Write(IntPtr commandPtr);
    public void Disconnect();
}