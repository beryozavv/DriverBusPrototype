using DriverBusPrototype.Agent;
using DriverBusPrototype.DriverCommands.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DriverBusPrototype.DriverCommands;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDriverBusServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<DriverBusSettings>(configuration.GetSection(nameof(DriverBusSettings)));

        services.AddSingleton<INativePort, NativePortMock>();
        services.AddSingleton<ICommunicationPort, CommunicationPort>();
        services.AddSingleton<ICommandExecutor, CommandExecutor>();
        services.AddSingleton<TaskCompletionDictionaryProvider>();
        services.AddScoped<IAgentService, AgentService>();
        services.AddHostedService<CommandResultBackgroundService>();

        return services;
    }
}