using DriverBusPrototype.DriverCommands;
using Xunit;
using Xunit.Abstractions;

namespace DriverBusPrototype.Tests;

public class CommandExecutorMockTests : IDisposable
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly ICommunicationPort _communicationPort;

    public CommandExecutorMockTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        INativePortMock nativePortMock = new NativePortMock(new LoggerMock<NativePortMock>(_testOutputHelper));
        _communicationPort =
            new CommunicationPortMock(new LoggerMock<CommunicationPortMock>(_testOutputHelper), nativePortMock);
        _communicationPort.Connect("TestPortName");
    }

    [Fact]
    public async Task SendParamsCommandAsyncTest()
    {
        ICommandExecutor commandExecutor =
            new CommandExecutorMock(_communicationPort, new LoggerMock<CommandExecutorMock>(_testOutputHelper));

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
    public async Task SendParamsCommandAsyncTimeoutExTest()
    {
        ICommandExecutor commandExecutor =
            new CommandExecutorMock(_communicationPort, new LoggerMock<CommandExecutorMock>(_testOutputHelper));

        var command = TestCommandsHelper.PrepareParamsCommand();

        var commandTimeoutException = await Assert.ThrowsAsync<TimeoutException>(async () =>
            await commandExecutor.ExecuteCommandAsync(command));

        _testOutputHelper.WriteLine($"A CommandTimeoutException was thrown {commandTimeoutException}");
    }

    public void Dispose()
    {
        _communicationPort.Dispose();
    }
}