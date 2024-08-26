using System.Text.Json.Serialization;
using DriverBusPrototype;
using DriverBusPrototypeService;
using DriverBusPrototypeService.Filters;
using Microsoft.AspNetCore.Http.Json;
using Xunit.Abstractions;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDriverBusServices();
builder.Services.AddSingleton<ITestOutputHelper, TestOutputHelper>();
builder.Services.AddScoped<ApiKeyAuthFilter>();
builder.Services.Configure<JsonOptions>(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
//builder.Services.AddHostedService<CommandResultBackgroundService>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();