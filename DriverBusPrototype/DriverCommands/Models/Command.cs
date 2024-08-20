using System.Runtime.InteropServices;

namespace DriverBusPrototype.DriverCommands.Models;

/// <summary> todo документация
/// 
/// </summary>
public struct Command
{
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    public string Id;

    [MarshalAs(UnmanagedType.U4)]
    public int Type;

    [MarshalAs(UnmanagedType.Bool)]
    public bool IsEncrypted;

    [MarshalAs(UnmanagedType.LPUTF8Str)]
    public string Parameters;
}