using CrossTech.DSS.Packages.Core.Models.Enums;

namespace DriverBusPrototype.Models;

public class PermissionsJson
{
    public string UserId { get; set; } = null!;

    public Dictionary<Guid, ePermissions>? MarkerPermisions { get; set; }
    public Dictionary<Guid, ePermissions>? EncryptionPermissions { get; set; }
}