using System.Text.Json;
using DriverBusPrototype.DriverCommands;
using DriverBusPrototype.DriverCommands.Models;
using Xunit;
using Xunit.Abstractions;

namespace DriverBusPrototype.Tests;

public class CommandExecutorTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public CommandExecutorTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    // [Fact]
    // public void SendParamsCommandTest()
    // {
    //     INativePortMock nativePortMock = new NativePortMock(TimeSpan.FromSeconds(2), _testOutputHelper);
    //     ICommunicationPort communicationPort = new CommunicationPortMock(_testOutputHelper, nativePortMock);
    //     ICommandExecutor commandExecutor = new CommandExecutor(communicationPort);
    //
    //     var command = PrepareCommand();
    //
    //     var commandResult = commandExecutor.ExecuteCommand(command);
    //
    //     if (!commandResult.IsSuccess)
    //     {
    //         _testOutputHelper.WriteLine($"ERROR in command response: {commandResult.ErrorCode} {commandResult.ErrorMessage}");
    //     }
    //     else
    //     {
    //         _testOutputHelper.WriteLine("Command was executed successfully");
    //     }
    //
    //     Assert.Equal(command.Id, commandResult.Id);
    // }
    
    [Fact]
    public async Task SendParamsCommandAsyncTest()
    {
        INativePortMock nativePortMock = new NativePortMock(_testOutputHelper);
        ICommunicationPort communicationPort = new CommunicationPortMock(_testOutputHelper, nativePortMock);
        ICommandExecutor commandExecutor = new CommandExecutor(communicationPort);

        var commandParams = PrepareParamsCommand();

        var commandPermissions = PreparePermissionsCommand();

        var permissionsTask = commandExecutor.ExecuteCommandAsync(commandPermissions);

        var paramsTask = commandExecutor.ExecuteCommandAsync(commandParams);

        var allResults = await Task.WhenAll(new[] { permissionsTask, paramsTask });

        _testOutputHelper.WriteLine("\n Выводим результаты выполнения команд:");
        foreach (var commandResult in allResults)
        {
            if (!commandResult.IsSuccess)
            {
                _testOutputHelper.WriteLine($"ERROR in command with id = {commandResult.Id} response: {commandResult.ErrorCode} {commandResult.ErrorMessage}");
            }
            else
            {
                _testOutputHelper.WriteLine($"Command was executed successfully {commandResult.Id}");
            }

            var ids = new[]{commandParams.Id, commandPermissions.Id};
            Assert.Contains(commandResult.Id, ids);
        }
    }

    // [Fact]
    // public void SendParamsCommandTimeoutExTest()
    // {
    //     INativePortMock nativePortMock = new NativePortMock(TimeSpan.FromSeconds(7), _testOutputHelper);
    //     ICommunicationPort communicationPort = new CommunicationPortMock(_testOutputHelper, nativePortMock);
    //     ICommandExecutor commandExecutor = new CommandExecutor(communicationPort);
    //
    //     var command = PrepareCommand();
    //
    //     Assert.Throws<CommandTimeoutException>(() =>
    //         commandExecutor.ExecuteCommand(command));
    //
    //     _testOutputHelper.WriteLine("A CommandTimeoutException was thrown");
    // }
    
    [Fact]
    public async Task SendParamsCommandAsyncTimeoutExTest()
    {
        INativePortMock nativePortMock = new NativePortMock(_testOutputHelper);
        ICommunicationPort communicationPort = new CommunicationPortMock(_testOutputHelper, nativePortMock);
        ICommandExecutor commandExecutor = new CommandExecutor(communicationPort);

        var command = PrepareParamsCommand();

        var commandTimeoutException = await Assert.ThrowsAsync<TimeoutException>(async () =>
            await commandExecutor.ExecuteCommandAsync(command));

        _testOutputHelper.WriteLine("A CommandTimeoutException was thrown");
    }

    private static Command PrepareParamsCommand()
    {
        var paramsJson = new ParamsJson
        {
            IsDriverEnabled = true,
            TrustedApps = new[] {new TrustedApp {AppName = "Word", Hash = "abc123Hash"}, new TrustedApp {AppName = "Acrobat", Hash = "qwe123Hash"}},
            EventsBatchSize = 10,
            FileFormats = new[] {FileFormat.Doc, FileFormat.Docx, FileFormat.Pdf, FileFormat.Xls, FileFormat.Xlsx}
        };

        var options = new JsonSerializerOptions {PropertyNameCaseInsensitive = true, WriteIndented = true};
        var json = JsonSerializer.Serialize(paramsJson, options);

        var command = new Command
        {
            Id = Guid.NewGuid().ToString(),
            IsEncrypted = false,
            Type = (int) CommandType.SetParams,
            Parameters = json
        };
        return command;
    }
    
    private static Command PreparePermissionsCommand()
    {
        var permissionsJson = new PermissionsJson
        {
            UserId = "123 My test SID 123",
            EncryptionPermissions = new Dictionary<Guid, int> {{Guid.NewGuid(), 2}, {Guid.NewGuid(), 4}, {Guid.NewGuid(), 8}},
            MarkerPermisions = new Dictionary<Guid, int> {{Guid.NewGuid(), 8}, {Guid.NewGuid(), 16}, {Guid.NewGuid(), 32}}
        };

        var options = new JsonSerializerOptions {PropertyNameCaseInsensitive = true, WriteIndented = true};
        var json = JsonSerializer.Serialize(permissionsJson, options);

        var command = new Command
        {
            Id = Guid.NewGuid().ToString(),
            IsEncrypted = false,
            Type = (int) CommandType.SetPermissions,
            Parameters = json
        };
        return command;
    }
}