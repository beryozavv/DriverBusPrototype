using System.Text.Json;
using CrossTech.DSS.Packages.Core.Models.Enums;
using DriverBusPrototype.DriverCommands;
using DriverBusPrototype.DriverCommands.Models;
using Xunit;
using Xunit.Abstractions;

namespace DriverBusPrototype.Tests;

public class CommandExecutorTests : IDisposable
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly ICommunicationPort _communicationPort;

    public CommandExecutorTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        INativePortMock nativePortMock = new NativePortMock(_testOutputHelper);
        _communicationPort = new CommunicationPortMock(_testOutputHelper, nativePortMock);
        _communicationPort.Connect("TestPortName");
    }

    [Fact]
    public async Task SendParamsCommandAsyncTest()
    {
        ICommandExecutor commandExecutor = new CommandExecutor(_communicationPort);

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
        ICommandExecutor commandExecutor = new CommandExecutor(_communicationPort);

        var command = PrepareParamsCommand();

        var commandTimeoutException = await Assert.ThrowsAsync<TimeoutException>(async () =>
            await commandExecutor.ExecuteCommandAsync(command));

        _testOutputHelper.WriteLine($"A CommandTimeoutException was thrown {commandTimeoutException}");
    }

    private static Command PrepareParamsCommand()
    {
        var paramsJson = new ParamsJson
        {
            IsDriverEnabled = true,
            TrustedApps = new[]
            {
                new TrustedApp { AppName = "Word", Hash = "abc123Hash" },
                new TrustedApp { AppName = "Acrobat", Hash = "qwe123Hash" }
            },
            EventsBatchSize = 10,
            FileFormats = new[] { FileFormat.Doc, FileFormat.Docx, FileFormat.Pdf, FileFormat.Xls, FileFormat.Xlsx }
        };

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, WriteIndented = true };
        var json = JsonSerializer.Serialize(paramsJson, options);

        var command = new Command
        {
            Id = Guid.NewGuid().ToString(),
            IsEncrypted = false,
            Type = (int)CommandType.SetParams,
            Parameters = json
        };
        return command;
    }

    private static Command PreparePermissionsCommand()
    {
        var permissionsJson = new PermissionsJson
        {
            UserId = "123 My test SID 123",
            EncryptionPermissions = new Dictionary<Guid, ePermissions> {{Guid.NewGuid(), ePermissions.Set}, {Guid.NewGuid(), ePermissions.Print}, {Guid.NewGuid(), ePermissions.Edit}},
            MarkerPermisions = new Dictionary<Guid, ePermissions> {{Guid.NewGuid(), ePermissions.SaveAs}, {Guid.NewGuid(), ePermissions.Edit}, {Guid.NewGuid(), ePermissions.CopyContent}}
        };

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, WriteIndented = true };
        var json = JsonSerializer.Serialize(permissionsJson, options);

        var command = new Command
        {
            Id = Guid.NewGuid().ToString(),
            IsEncrypted = false,
            Type = (int)CommandType.SetPermissions,
            Parameters = json
        };
        return command;
    }

    public void Dispose()
    {
        _communicationPort.Dispose();
    }
}