using System.Reflection;
using DriverBusPrototype.Models;
using DriverBusPrototype.Services;
using DriverBusPrototype.Streams;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DriverBusPrototype;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDriverBusServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<DriverBusSettings>(configuration.GetSection(nameof(DriverBusSettings)));

        services.AddSingleton<IInputCommunicationStream, InputCommunicationStream>();
        services.AddSingleton<IOutputCommunicationStream, OutputCommunicationStream>();
        services.AddSingleton<ICommandExecutor, CommandExecutor>();
        services.AddSingleton<TaskCompletionDictionaryProvider>();
        services.AddSingleton<ICommandResultService, CommandResultService>();
        services.AddSingleton<IRequestExecutor, RequestExecutor>();
        services.AddHostedService<CommandsBackgroundService>();
        
        services.AddMediatR(Assembly.GetAssembly(typeof(Command))!);

        return services;
    }
}