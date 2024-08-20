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

    [Fact]
    public void SendParamsCommandTest()
    {
        ICommunicationPort communicationPort = new CommunicationPortMock(_testOutputHelper, TimeSpan.FromSeconds(2));
        ICommandExecutor commandExecutor = new CommandExecutor(communicationPort);

        var command = PrepareCommand();

        var commandResult = commandExecutor.SendCommand(command);

        if (!commandResult.IsSuccess)
        {
            _testOutputHelper.WriteLine($"ERROR in response: {commandResult.ErrorCode} {commandResult.ErrorMessage}");
        }
        else
        {
            _testOutputHelper.WriteLine("Command was executed successfully");
        }

        Assert.Equal(command.Id, commandResult.Id);
    }

    [Fact]
    public void SendParamsCommandTimeoutExTest()
    {
        ICommunicationPort communicationPort = new CommunicationPortMock(_testOutputHelper, TimeSpan.FromSeconds(7));
        ICommandExecutor commandExecutor = new CommandExecutor(communicationPort);

        var command = PrepareCommand();

        Assert.Throws<CommandTimeoutException>(() =>
            commandExecutor.SendCommand(command));

        _testOutputHelper.WriteLine("A CommandTimeoutException was thrown");
    }

    private static Command PrepareCommand()
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
}