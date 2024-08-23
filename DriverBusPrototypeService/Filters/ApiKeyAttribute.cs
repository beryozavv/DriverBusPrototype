using Microsoft.AspNetCore.Mvc;

namespace DriverBusPrototypeService.Filters;

/// <summary>
/// Класс атрибута авторизации по ApiKey
/// </summary>
public class ApiKeyAttribute : ServiceFilterAttribute
{
    /// <summary>
    /// атрибут авторизации по ApiKey
    /// </summary>
    public ApiKeyAttribute()
        : base(typeof(ApiKeyAuthFilter))
    {
    }
}