namespace DriverBusPrototype.Agent.Auth
{
    /// <summary>
    /// Http-обработчик с политикой повторов при неудачной авторизации
    /// </summary>
    public class AuthHttpHandler : HttpClientHandler
    {
        private const string ApikeyHeader = "X-API-KEY";

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            if (request.Headers.Contains(ApikeyHeader))
            {
                request.Headers.Remove(ApikeyHeader);
            }

            request.Headers.Add(ApikeyHeader, new AuthContext().GetApiKey());

            var response = await base.SendAsync(request, ct);

            return response;
        }
    }
}