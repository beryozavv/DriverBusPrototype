using DriverBusPrototype;
using DriverBusPrototypeService;
using Xunit.Abstractions;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDriverBusServices();
builder.Services.AddSingleton<ITestOutputHelper, TestOutputHelper>();
//builder.Services.AddHostedService<CommandResultBackgroundService>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();