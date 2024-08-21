using System.Net;
using System.Text;
using System.Text.Json;
using CrossTech.DSS.Packages.Core.Models.DTO.Event;
using DriverBusPrototype.Agent.Auth;

namespace DriverBusPrototype.Agent;

internal class AgentService : IAgentService
{
    public async Task<string> GetEncryptionKey(Guid documentGuid)
    {
        var getEncryptionKeyClientUrl = string.Format("http://127.0.0.1:5000/api/Encryption/{{0}}/encryptionKey", documentGuid);

        using (var client = new HttpClient(new AuthHttpHandler()))
        {
            var encryptionKey = await client.GetStringAsync(new Uri(getEncryptionKeyClientUrl));

            return encryptionKey;
        }
    }

    public async Task<long> SendEventsBatch(EventDto[] events)
    {
        using (var client = new HttpClient(new AuthHttpHandler()))
        {
            var jsContent = JsonSerializer.Serialize(events);

            var content = new StringContent(jsContent, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("http://127.0.0.1:5000/api/SaveEvents", content);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return 1; //todo read result
            }
            else
            {
                throw new Exception("error" + response.StatusCode);
            }
        }
    }
}