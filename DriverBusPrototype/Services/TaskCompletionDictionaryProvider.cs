using System.Collections.Concurrent;
using DriverBusPrototype.Models;

namespace DriverBusPrototype.Services;

/// <summary>
/// Для инъекции словаря
/// </summary>
public class TaskCompletionDictionaryProvider
{
    public ConcurrentDictionary<Guid, TaskCompletionSource<CommandResult>>
        TaskCompletionSourcesDict { get; } = new();
}