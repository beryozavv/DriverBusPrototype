using CrossTech.DSS.Packages.Core.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DriverBusPrototypeService.Filters;

/// <summary>
/// Фильтр авторизации запросов по API Key
/// </summary>
public class ApiKeyAuthFilter : IAuthorizationFilter
{
    private readonly ILogger<ApiKeyAuthFilter> _logger;

    public ApiKeyAuthFilter(ILogger<ApiKeyAuthFilter> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Проверка входящих запросов на наличие валидного API KEY
    /// </summary>
    /// <param name="context"> Контекст </param>
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        AuthContext authContext = new AuthContext();

        string apiKey = context.HttpContext.Request.Headers[AuthContext.API_KEY_HEADER];

        if (!authContext.IsValidApiKey(apiKey))
        {
            _logger.LogDebug("Invalid API Key");

            context.Result = new UnauthorizedResult();
        }
    }
}