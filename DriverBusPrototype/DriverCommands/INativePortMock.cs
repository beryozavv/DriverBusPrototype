namespace DriverBusPrototype.DriverCommands;

public interface INativePortMock
{
    public bool Connect(string portName);
    public IntPtr Read();   
    public void Write(IntPtr commandPtr);
    public void Disconnect();
}