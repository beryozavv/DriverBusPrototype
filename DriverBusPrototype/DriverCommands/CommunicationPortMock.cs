using System.Runtime.InteropServices;
using Xunit.Abstractions;

namespace DriverBusPrototype.DriverCommands;

internal class CommunicationPortMock : ICommunicationPort
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly INativePortMock _nativePortMock;

    public CommunicationPortMock(ITestOutputHelper testOutputHelper, INativePortMock nativePortMock)
    {
        _testOutputHelper = testOutputHelper;
        _nativePortMock = nativePortMock;
    }

    public bool Connect(string portName)
    {
        return _nativePortMock.Connect(portName);
    }

    public T Read<T>() where T : struct
    {
        _testOutputHelper.WriteLine("Начинаем считывание результатов команд из драйвера");
        var resultPtr = _nativePortMock.Read();

        var commandResult = Marshal.PtrToStructure<T>(resultPtr);

        return commandResult;
    }

    public void Write<T>(T command) where T : struct
    {
        var commandPtr =
            StructureToPtrHelper.GetStructurePtr(command);

        _testOutputHelper.WriteLine("Отправляем в драйвер указатель на команду commandPtr = " + commandPtr);

        _nativePortMock.Write(commandPtr);
    }

    public void Disconnect()
    {
        _nativePortMock.Disconnect();
    }
}