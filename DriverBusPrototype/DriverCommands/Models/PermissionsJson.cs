namespace DriverBusPrototype.DriverCommands.Models;

public class PermissionsJson
{
    public string UserId { get; set; } = null!;

    public Dictionary<Guid, int>? MarkerPermisions { get; set; }
    public Dictionary<Guid, int>? EncryptionPermissions { get; set; }
}