using System.Runtime.InteropServices;

namespace DriverBusPrototype.DriverCommands;

public static class StructureToPtrHelper
{
    public static IntPtr GetCommandPtr<T>(T command) where T : struct
    {
        var size = Marshal.SizeOf(command);
        var ptr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(command, ptr, false);

        return ptr;
    }
}