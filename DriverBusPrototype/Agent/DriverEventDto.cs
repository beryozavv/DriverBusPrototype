using System.Runtime.Serialization;
using CrossTech.DSS.Packages.Core.Models.Enums;

namespace DriverBusPrototype.Agent;

[DataContract]
public class DriverEventDto
{
    [DataMember] public long DriverEventId { get; set; }

    [DataMember] public eEventType? EventType { get; set; }

    [DataMember] public DateTime? EventDateTimeUtc { get; set; }

    [DataMember] public Guid? DocGuid { get; set; }

    [DataMember] public Guid? ParentGuid { get; set; }

    [DataMember] public string DocType { get; set; } = null!;

    [DataMember] public string DocAuthor { get; set; } = null!;

    [DataMember] public Guid? MarkerGuid { get; set; }

    [DataMember] public int? EncryptionPolicyId { get; set; }

    [DataMember] public string FileName { get; set; } = null!;

    [DataMember] public string FilePath { get; set; } = null!;

    [DataMember] public string? ParentFileName { get; set; }

    [DataMember] public string? ParentFilePath { get; set; }

    [DataMember] public string UserId { get; set; } = null!;

    [DataMember] public string? UserName { get; set; }
}