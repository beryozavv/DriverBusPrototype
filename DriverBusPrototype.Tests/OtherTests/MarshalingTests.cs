using System.Runtime.InteropServices;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS8601

namespace DriverBusPrototype.Tests.OtherTests;

public class MarshalingTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public MarshalingTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MyStruct
    {
        public int intValue;
        public IntPtr stringPtr; // Pointer to the string

        [MarshalAs(UnmanagedType.LPStr)] public string stringValue; // Managed string for easier handling
    }

    public static IntPtr MarshalToUnmanagedMemory(MyStruct myStruct)
    {
        int size = Marshal.SizeOf(myStruct);
        IntPtr ptr = Marshal.AllocHGlobal(size);

        // Allocate unmanaged memory for the string and copy the string to it
        myStruct.stringPtr = Marshal.StringToHGlobalUni(myStruct.stringValue);

        // Copy the struct to unmanaged memory
        Marshal.StructureToPtr(myStruct, ptr, false);

        return ptr;
    }

    private static MyStruct MarshalFromUnmanagedMemory(IntPtr ptr)
    {
        // Marshal the unmanaged memory to the struct
        MyStruct myStruct = Marshal.PtrToStructure<MyStruct>(ptr);

        // Get the string from unmanaged memory
        myStruct.stringValue = Marshal.PtrToStringUni(myStruct.stringPtr);

        return myStruct;
    }

    private static void FreeUnmanagedMemory(IntPtr ptr)
    {
        // Marshal the struct to get the pointer to the string
        MyStruct myStruct = Marshal.PtrToStructure<MyStruct>(ptr);

        // Free the unmanaged memory for the string
        Marshal.FreeHGlobal(myStruct.stringPtr);

        // Free the unmanaged memory for the struct
        Marshal.FreeHGlobal(ptr);
    }

    [Fact]
    public void MarshalingStringTest()
    {
        MyStruct myStruct = new MyStruct
        {
            intValue = 42,
            stringValue = "Hello, World! Привет, Мир!!! @"
        };

        IntPtr ptr = MarshalToUnmanagedMemory(myStruct);
        _testOutputHelper.WriteLine("Struct marshalled to unmanaged memory.");

        MyStruct newStruct = MarshalFromUnmanagedMemory(ptr);
        _testOutputHelper.WriteLine($"intValue: {newStruct.intValue}, stringValue: {newStruct.stringValue}");

        FreeUnmanagedMemory(ptr);
        _testOutputHelper.WriteLine("Unmanaged memory freed.");
    }
}