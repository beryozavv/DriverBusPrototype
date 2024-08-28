namespace DriverBusPrototype.Tests;

public static class Settings
{
    public static TimeSpan CommandTimeout => TimeSpan.FromSeconds(3);
    public static TimeSpan ReadTimout => TimeSpan.FromMilliseconds(100);

    public static bool NativePortMockExceptions { get; set; } = true;
}