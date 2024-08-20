using DriverBusPrototype.AgentService.Auth;

namespace DriverBusPrototype.AgentService;

public class AgentService : IAgentService
{
    public async Task<string> GetEncryptionKey(Guid documentGuid)
    {
        var getEncryptionKeyClientUrl = string.Format(Settings.ClientServiceGetEncryptionKeyUrl, documentGuid);

        using (HttpClient client = new HttpClient(new AuthHttpHandler()))
        {
            var encryptionKey = await client.GetStringAsync(new Uri(getEncryptionKeyClientUrl));

            return encryptionKey;
        }
    }
}