using System.Net;
using System.Text;
using System.Text.Json;
using DriverBusPrototype.Agent.Auth;

namespace DriverBusPrototype.Agent;

internal class AgentService : IAgentService
{
    public async Task<string> GetEncryptionKey(Guid documentGuid)
    {
        var getEncryptionKeyClientUrl = string.Format("http://127.0.0.1:5000/api/Encryption/{0}/encryptionKey", documentGuid);

        using (var client = new HttpClient(new AuthHttpHandler()))
        {
            var encryptionKey = await client.GetStringAsync(new Uri(getEncryptionKeyClientUrl));

            return encryptionKey;
        }
    }

    public async Task<long> SendEventsBatch(DriverEventDto[] events)
    {
        using (var client = new HttpClient(new AuthHttpHandler()))
        {
            var jsContent = JsonSerializer.Serialize(events);

            var content = new StringContent(jsContent, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("http://127.0.0.1:5000/api/SaveEvents", content);

            var responseText = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (long.TryParse(responseText, out var responseMaxId))
                {
                    return responseMaxId;
                }
                return 0;
            }
            else
            {
                throw new Exception("error" + response.StatusCode + " " + responseText);
            }
        }
    }
}