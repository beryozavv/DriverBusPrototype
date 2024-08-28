using System.Text.Json.Serialization;
using DriverBusPrototype;
using DriverBusPrototypeService;
using DriverBusPrototypeService.Filters;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDriverBusServices();
builder.Services.AddScoped<ApiKeyAuthFilter>();
builder.Services.Configure<JsonOptions>(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddHostedService<CommandResultBackgroundService>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

// app.Lifetime.ApplicationStarted.Register(OnStarted);
// app.Lifetime.ApplicationStopped.Register(OnStopped);

app.Run();

// void OnStopped()
// {
//     var communicationPort = app.Services.GetRequiredService<ICommunicationPort>();
//     communicationPort.Dispose();
// }
//
//
// void OnStarted()
// {
//     var communicationPort = app.Services.GetRequiredService<ICommunicationPort>();
//     communicationPort.Connect("test port todo from config"); // todo
// }