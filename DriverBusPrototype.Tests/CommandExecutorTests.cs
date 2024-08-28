using DriverBusPrototype.DriverCommands;
using DriverBusPrototype.DriverCommands.Helpers;
using DriverBusPrototype.DriverCommands.Services;
using DriverBusPrototype.Tests.Mocks;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace DriverBusPrototype.Tests;

public class CommandExecutorTests : IDisposable
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly ICommunicationPort _communicationPort;
    private readonly Mock<IOptions<DriverBusSettings>> _settingsMock;

    public CommandExecutorTests(ITestOutputHelper testOutputHelper)
    {
        _settingsMock = new Mock<IOptions<DriverBusSettings>>();
        _settingsMock.Setup(o => o.Value).Returns(new DriverBusSettings
        {
            CommandTimeout = Settings.CommandTimeout, ReadTimeout = Settings.ReadTimout,
            NativePortMockExceptions = Settings.NativePortMockExceptions
        });

        _testOutputHelper = testOutputHelper;
        INativePort nativePort =
            new NativePortMock(new LoggerMock<NativePortMock>(_testOutputHelper), _settingsMock.Object);
        _communicationPort =
            new CommunicationPort(new LoggerMock<CommunicationPort>(_testOutputHelper), nativePort);
        _communicationPort.Connect("TestPortName");
    }

    [Fact]
    public async Task SendParamsCommandTest()
    {
        ICommandExecutor commandExecutor =
            new CommandExecutorMock(_communicationPort, new LoggerMock<CommandExecutorMock>(_testOutputHelper),
                _settingsMock.Object);

        var commandParams = TestCommandsHelper.PrepareParamsCommand();

        var commandPermissions = TestCommandsHelper.PreparePermissionsCommand();

        var permissionsTask = commandExecutor.ExecuteCommandAsync(commandPermissions);

        var paramsTask = commandExecutor.ExecuteCommandAsync(commandParams);

        var allResults = await Task.WhenAll(new[] { permissionsTask, paramsTask });

        _testOutputHelper.WriteLine("\n Выводим результаты выполнения команд:");
        foreach (var commandResult in allResults)
        {
            if (!commandResult.IsSuccess)
            {
                _testOutputHelper.WriteLine(
                    $"ERROR in command with id = {commandResult.Id} response: {commandResult.ErrorCode} {commandResult.ErrorMessage}");
            }
            else
            {
                _testOutputHelper.WriteLine($"Command was executed successfully {commandResult.Id}");
            }

            var ids = new[] { commandParams.Id, commandPermissions.Id };
            Assert.Contains(commandResult.Id, ids);
        }
    }

    [Fact]
    public async Task SendParamsCommandTimeoutExTest()
    {
        var localSettingsMock = new Mock<IOptions<DriverBusSettings>>();
        localSettingsMock.Setup(o => o.Value).Returns(new DriverBusSettings
        {
            CommandTimeout = TimeSpan.FromSeconds(1), ReadTimeout = TimeSpan.FromSeconds(5),
            NativePortMockExceptions = false
        });
        
        INativePort nativePort =
            new NativePortMock(new LoggerMock<NativePortMock>(_testOutputHelper), localSettingsMock.Object);
        var communicationPort =
            new CommunicationPort(new LoggerMock<CommunicationPort>(_testOutputHelper), nativePort);
        communicationPort.Connect("TestPortName");

        ICommandExecutor commandExecutor =
            new CommandExecutorMock(communicationPort, new LoggerMock<CommandExecutorMock>(_testOutputHelper),
                localSettingsMock.Object);

        var command = TestCommandsHelper.PrepareParamsCommand();

        var commandTimeoutException = await Assert.ThrowsAsync<TimeoutException>(async () =>
            await commandExecutor.ExecuteCommandAsync(command));

        _testOutputHelper.WriteLine($"A TimeoutException was thrown: {commandTimeoutException}");
    }

    [Fact]
    public async Task SendParamsCommandCancellationTest()
    {
        ICommandExecutor commandExecutor =
            new CommandExecutorMock(_communicationPort, new LoggerMock<CommandExecutorMock>(_testOutputHelper),
                _settingsMock.Object);

        var command = TestCommandsHelper.PrepareParamsCommand();

        var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(20));

        var commandTimeoutException = await Assert.ThrowsAsync<TaskCanceledException>(async () =>
            await commandExecutor.ExecuteCommandAsync(command, cts.Token));

        _testOutputHelper.WriteLine($"A CommandTimeoutException was thrown {commandTimeoutException}");
    }

    public void Dispose()
    {
        _communicationPort.Dispose();
    }
}