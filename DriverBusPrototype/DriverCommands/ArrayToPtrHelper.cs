using System.Runtime.InteropServices;

namespace DriverBusPrototype.DriverCommands;

public static class ArrayToPtrHelper
{
    public static IntPtr ByteArrayToPtr(byte[] bytes)
    {
        var unmanagedPointer = Marshal.AllocHGlobal(bytes.Length);
        Marshal.Copy(bytes, 0, unmanagedPointer, bytes.Length);

        return unmanagedPointer;
    }
    
    public static byte[] ByteArrayFromPtr(IntPtr dataPtr, int length)
    {
        var byteArray = new byte[length];
        Marshal.Copy(dataPtr, byteArray, 0, length);

        return byteArray;
    } 
}