using System.Collections.Concurrent;
using DriverBusPrototype.DriverCommands.Models;

namespace DriverBusPrototype.DriverCommands.Services;

public class TaskCompletionDictionaryProvider
{
    public ConcurrentDictionary<string, TaskCompletionSource<CommandResult>>
        TaskCompletionSourcesDict { get; } = new();
}