namespace DriverBusPrototype.DriverCommands;

public interface INativePortMock
{
    public bool Connect(string portName);
    public void Read(out IntPtr dataPtr, out int dataSize); // todo Здесь мы задаем буфер?   
    public void Write(IntPtr commandPtr, int size);
    public void Disconnect();
}