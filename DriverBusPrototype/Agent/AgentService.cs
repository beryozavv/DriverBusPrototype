using System.Net;
using System.Text;
using System.Text.Json;
using CrossTech.DSS.Packages.Core.Models.DTO.Event;
using DriverBusPrototype.Agent.Auth;

namespace DriverBusPrototype.Agent;

public class AgentService : IAgentService
{
    public async Task<string> GetEncryptionKey(Guid documentGuid)
    {
        var getEncryptionKeyClientUrl = string.Format(Settings.ClientServiceGetEncryptionKeyUrl, documentGuid);

        using (var client = new HttpClient(new AuthHttpHandler()))
        {
            var encryptionKey = await client.GetStringAsync(new Uri(getEncryptionKeyClientUrl));

            return encryptionKey;
        }
    }

    public async Task<bool> SendEventsBatch(EventDto[] events)
    {
        using (var client = new HttpClient(new AuthHttpHandler()))
        {
            var jsContent = JsonSerializer.Serialize(events);

            var content = new StringContent(jsContent, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(Settings.ClientServicePostEventsUrl, content);

            return response.StatusCode == HttpStatusCode.OK;
        }
    }
}