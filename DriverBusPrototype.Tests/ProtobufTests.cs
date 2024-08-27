using System.Text.Json;
using System.Text.Json.Serialization;
using CrossTech.DSS.Packages.Core.Models.Enums;
using DriverBusPrototype.DriverCommands;
using DriverBusPrototype.DriverCommands.Models;
using Xunit;
using Xunit.Abstractions;

namespace DriverBusPrototype.Tests;

public class ProtobufTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public ProtobufTests(ITestOutputHelper _testOutputHelper)
    {
        this._testOutputHelper = _testOutputHelper;
    }
    
    [Fact]
    public async Task ProtobufParamsCommandToUnmanagedTest()
    {
        var command = PrepareParamsCommand();

        var (intPtr, size) = ProtoConverter.ObjectToProtoPtr(command);

        var newCommand = ProtoConverter.ObjectFromProtoPtr<Command>(intPtr, size);
        
        Assert.Equal(command.Id, newCommand.Id);
        Assert.Equal(command.Type, newCommand.Type);
        Assert.Equal(command.Parameters, newCommand.Parameters);
    }
    
    [Fact]
    public async Task ProtobufPermissionCommandToUnmanagedTest()
    {
        var command = PreparePermissionsCommand();

        var (intPtr, size) = ProtoConverter.ObjectToProtoPtr(command);

        var newCommand = ProtoConverter.ObjectFromProtoPtr<Command>(intPtr, size);
        
        Assert.Equal(command.Id, newCommand.Id);
        Assert.Equal(command.Type, newCommand.Type);
        Assert.Equal(command.Parameters, newCommand.Parameters);
    }
    
    [Fact]
    public async Task ProtobufCommandResultToUnmanagedTest()
    {
        var command = GetTestResult("123123484asdgfasdfg456");

        var (intPtr, size) = ProtoConverter.ObjectToProtoPtr(command);

        var newCommand = ProtoConverter.ObjectFromProtoPtr<CommandResult>(intPtr, size);
        
        Assert.Equal(command.Id, newCommand.Id);
        Assert.Equal(command.ErrorCode, newCommand.ErrorCode);
        Assert.Equal(command.ErrorMessage, newCommand.ErrorMessage);
    }
    
    private CommandResult GetTestResult(string commandId)
    {
        return new CommandResult
        {
            Id = commandId,
            IsSuccess = false,
            ErrorCode = 123123,
            ErrorMessage =
                $"Error526526 Error123123 Error123123 Error123123 Error123123 Error123123 Error123123 Error123123 Error123123 commandId = {commandId}"
        };
    }

    private Command PrepareParamsCommand()
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

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, WriteIndented = true, Converters = { new JsonStringEnumConverter() } };
        var json = JsonSerializer.Serialize(paramsJson, options);
        
        _testOutputHelper.WriteLine(json);

        var command = new Command
        {
            Id = Guid.NewGuid().ToString(),
            IsEncrypted = false,
            Type = CommandType.SetParams,
            Parameters = json
        };
        return command;
    }

    private Command PreparePermissionsCommand()
    {
        var permissionsJson = new PermissionsJson
        {
            UserId = "123 My test SID 123",
            EncryptionPermissions = new Dictionary<Guid, ePermissions> {{Guid.NewGuid(), ePermissions.Set|ePermissions.Open}, {Guid.NewGuid(), ePermissions.Print}, {Guid.NewGuid(), ePermissions.Edit|ePermissions.Screenshot}},
            MarkerPermisions = new Dictionary<Guid, ePermissions> {{Guid.NewGuid(), ePermissions.SaveAs}, {Guid.NewGuid(), ePermissions.Edit}, {Guid.NewGuid(), ePermissions.CopyContent|ePermissions.Print|ePermissions.SetMarkerWithLowerCriticalLevel}}
        };

        var options = new JsonSerializerOptions {PropertyNameCaseInsensitive = true, WriteIndented = true, Converters = { new JsonStringEnumConverter() }};
        var json = JsonSerializer.Serialize(permissionsJson, options);
        
        _testOutputHelper.WriteLine(json);

        var command = new Command
        {
            Id = Guid.NewGuid().ToString(),
            Type = CommandType.SetPermissions,
            IsEncrypted = false,
            Parameters = json
        };

        return command;
    }
}