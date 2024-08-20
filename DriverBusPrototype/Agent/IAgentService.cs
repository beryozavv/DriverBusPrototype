using CrossTech.DSS.Packages.Core.Models.DTO.Event;

namespace DriverBusPrototype.Agent;

public interface IAgentService
{
    Task<string> GetEncryptionKey(Guid documentGuid);

    Task<bool> SendEventsBatch(EventDto[] events);
}