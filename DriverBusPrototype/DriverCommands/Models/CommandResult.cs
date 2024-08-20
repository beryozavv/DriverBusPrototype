using System.Runtime.InteropServices;

namespace DriverBusPrototype.DriverCommands.Models;

public struct CommandResult
{
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    public string Id;

    [MarshalAs(UnmanagedType.Bool)]
    public bool IsSuccess;

    [MarshalAs(UnmanagedType.U4)]
    public int ErrorCode;

    [MarshalAs(UnmanagedType.LPUTF8Str)]
    public string ErrorMessage;
}