namespace DriverBusPrototype.AgentService;

public interface IAgentService
{
    Task<string> GetEncryptionKey(Guid documentGuid);

    //bool SendEvent(EventDto eventObj); todo EventDto содержит много лишних полей и вложенностей
}