using System.Runtime.InteropServices;
using Xunit;
using Xunit.Abstractions;

namespace DriverBusPrototype.Tests.OtherTests;

public class StringMarshalerTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public StringMarshalerTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MyStruct
    {
        [MarshalAs(UnmanagedType.U4)]
        public int intValue;

        //[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaler))]
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public string stringValue; // Используем наш пользовательский маршализатор
    }

    [Fact]
    public void MarshalingCustomTest()
    {
        MyStruct myStruct = new MyStruct
        {
            intValue = 125,
            stringValue = "Hello, World! Привет, Мир!!! @ Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @" +
                          "Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @" +
                          "Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @" +
                          "Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @" +
                          "Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @" +
                          "Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! @Hello, World! Привет, Мир!!! конец строки ✅ @"
        };

        int size = Marshal.SizeOf(myStruct);
        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(myStruct, ptr, false);

        // Marshal the unmanaged memory to the struct
        MyStruct myStructNew = Marshal.PtrToStructure<MyStruct>(ptr);

        _testOutputHelper.WriteLine($"{myStructNew.stringValue + " - " + myStructNew.intValue}");
    }
}