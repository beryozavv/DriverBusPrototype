using System.Text.Json;
using DriverBusPrototype.Models;
using MediatR;

namespace DriverBusPrototype.Handlers;

public class BaseDriverRequest : IRequest
{
    public Guid Id { get; init; }
    public CommandType CommandType { get; init; }
}

public class BaseDriverRequest<T> : BaseDriverRequest
{
    public T RequestBody { get; init; } = default!;

    public static T? DeserializeRequestBody(string json)
    {
        return JsonSerializer.Deserialize<T>(json, BusJsonOptions.GetOptions());
    }
}