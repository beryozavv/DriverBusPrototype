namespace DriverBusPrototype;

public static class Settings
{
    public static string ClientServiceGetEncryptionKeyUrl => "url";

    public static TimeSpan CommandTimeout => TimeSpan.FromSeconds(3);
}