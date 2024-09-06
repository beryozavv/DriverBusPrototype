using System.Text.Json;
using System.Text.Json.Serialization;

namespace DriverBusPrototype;

public static class BusJsonOptions
{
    public static JsonSerializerOptions GetOptions()
    {
        return new JsonSerializerOptions
        {
            WriteIndented = false,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }
}