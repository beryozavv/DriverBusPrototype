namespace DriverBusPrototype;

public static class Settings
{
    public static string ClientServiceGetEncryptionKeyUrl => "http://127.0.0.1:5000/api/Encryption/{0}/encryptionKey";
    public static string ClientServicePostEventsUrl => "http://127.0.0.1:5000/api/SaveEvents";

    public static TimeSpan CommandTimeout => TimeSpan.FromSeconds(3);
}