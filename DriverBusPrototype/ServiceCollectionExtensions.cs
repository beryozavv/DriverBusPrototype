using DriverBusPrototype.Agent;
using DriverBusPrototype.DriverCommands;
using Microsoft.Extensions.DependencyInjection;

namespace DriverBusPrototype;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDriverBusServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<INativePortMock, NativePortMock>();
        serviceCollection.AddSingleton<ICommunicationPort, CommunicationPortMock>();
        serviceCollection.AddSingleton<ICommandExecutor, CommandExecutor>();
        serviceCollection.AddSingleton<TaskCompletionDictionaryProvider>();
        serviceCollection.AddScoped<IAgentService, AgentService>();
        
        return serviceCollection;
    }
}