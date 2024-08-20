namespace DriverBusPrototype.AgentService.Auth
{
    /// <summary>
    /// Http-обработчик с политикой повторов при неудачной авторизации
    /// </summary>
    public class AuthHttpHandler : HttpClientHandler
    {
        private const string APIKEY_HEADER = "X-API-KEY";

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            if (request.Headers.Contains(APIKEY_HEADER))
            {
                request.Headers.Remove(APIKEY_HEADER);
            }

            request.Headers.Add(APIKEY_HEADER, new AuthContext().GetApiKey());

            var response = await base.SendAsync(request, ct);

            return response;
        }
    }
}