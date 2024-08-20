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
        ISocket socket = new SocketMock(_testOutputHelper);
        ICommandExecutor commandExecutor = new CommandExecutor(socket);

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

        var commandResult = commandExecutor.SendCommand(command);

        if (!commandResult.IsSuccess)
        {
            _testOutputHelper.WriteLine($"ERROR: {commandResult.ErrorCode} {commandResult.ErrorMessage}");
        }
        else
        {
            _testOutputHelper.WriteLine("Command was executed successfully");
        }
    }
}