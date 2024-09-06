namespace DriverBusPrototype.Models;

public enum CommandType
{
    Default = 0,
    SetParams = 1,
    SetPermissions = 2,
    GetEncryptionKey = 3,
    SendEventsBatch = 4
}