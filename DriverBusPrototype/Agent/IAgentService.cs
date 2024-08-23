namespace DriverBusPrototype.Agent;

public interface IAgentService
{
    Task<string> GetEncryptionKey(Guid documentGuid);

    Task<long> SendEventsBatch(DriverEventDto[] events);
}