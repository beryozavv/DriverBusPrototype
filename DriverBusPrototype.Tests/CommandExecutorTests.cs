using DriverBusPrototype.Services;
using DriverBusPrototype.Streams;
using DriverBusPrototype.Tests.Mocks;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace DriverBusPrototype.Tests;

public class CommandExecutorTests : IDisposable
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IOutputCommunicationStream _communicationStream;
    private readonly Mock<IOptions<DriverBusSettings>> _settingsMock;

    public CommandExecutorTests(ITestOutputHelper testOutputHelper)
    {
        _settingsMock = new Mock<IOptions<DriverBusSettings>>();
        _settingsMock.Setup(o => o.Value).Returns(new DriverBusSettings
        {
            CommandTimeout = Settings.CommandTimeout, ReadTimeout = Settings.ReadTimout,
            NativePortMockExceptions = Settings.NativePortMockExceptions,
            InputPipeName = "pipe1", OutputPipeName = "pipe2"
        });

        _testOutputHelper = testOutputHelper;
        _communicationStream =
            new OutputCommunicationStream(new LoggerMock<OutputCommunicationStream>(_testOutputHelper), _settingsMock.Object);
        _communicationStream.ConnectAsync().GetAwaiter().GetResult();
    }

    [Fact]
    public async Task SendParamsCommandTest()
    {
        ICommandExecutor commandExecutor =
            new CommandExecutorMock(_communicationStream, new LoggerMock<CommandExecutorMock>(_testOutputHelper),
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
            NativePortMockExceptions = false,
            InputPipeName = "pipe1", OutputPipeName = "pipe2"
        });

        IOutputCommunicationStream communicationStream =
            new OutputCommunicationStream(new LoggerMock<OutputCommunicationStream>(_testOutputHelper), localSettingsMock.Object);

        await communicationStream.ConnectAsync();

        ICommandExecutor commandExecutor =
            new CommandExecutorMock(communicationStream, new LoggerMock<CommandExecutorMock>(_testOutputHelper),
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
            new CommandExecutorMock(_communicationStream, new LoggerMock<CommandExecutorMock>(_testOutputHelper),
                _settingsMock.Object);

        var command = TestCommandsHelper.PrepareParamsCommand();

        var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(20));
        cts.Cancel();
        var commandTimeoutException = await Assert.ThrowsAsync<TaskCanceledException>(async () =>
            await commandExecutor.ExecuteCommandAsync(command, cts.Token));

        _testOutputHelper.WriteLine($"A CommandTimeoutException was thrown {commandTimeoutException}");
    }

    public void Dispose()
    {
        _communicationStream.Dispose();
    }
}