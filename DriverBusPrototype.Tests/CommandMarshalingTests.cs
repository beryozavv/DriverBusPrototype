using System.Runtime.InteropServices;
using System.Text.Json;
using DriverBusPrototype.DriverCommands.Models;
using Xunit;
using Xunit.Abstractions;

namespace DriverBusPrototype.Tests;

public class CommandMarshalingTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public CommandMarshalingTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void SetPermissionsTest()
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
            Type = (int) CommandType.SetPermissions,
            IsEncrypted = false,
            Parameters = json
        };

        var size = Marshal.SizeOf(command);
        var ptr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(command, ptr, false);

        // Marshal the unmanaged memory to the struct
        var commandNew = Marshal.PtrToStructure<Command>(ptr);

        var permissionsJsonNew = JsonSerializer.Deserialize<PermissionsJson>(commandNew.Parameters);

        _testOutputHelper.WriteLine($"{commandNew.Id + " - " + (CommandType) commandNew.Type + " - " + commandNew.Parameters}");

        Assert.Equal(command.Parameters, commandNew.Parameters);
        Assert.Equal(command.Id, commandNew.Id);
        Assert.Equal(command.Type, commandNew.Type);

        Assert.Equal(permissionsJson.UserId, permissionsJsonNew!.UserId);
    }

    [Fact]
    public void SetParamsTest()
    {
        var paramsJson = new ParamsJson
        {
            IsDriverEnabled = true,
            TrustedApps = new[] {new TrustedApp {AppName = "Word", Hash = "abc123Hash"}, new TrustedApp {AppName = "Acrobat", Hash = "qwe123Hash"}},
            EventsBatchSize = 10,
            FileFormats = new[] {FileFormat.Doc, FileFormat.Docx, FileFormat.Pdf}
        };

        var options = new JsonSerializerOptions {PropertyNameCaseInsensitive = true, WriteIndented = true};
        var json = JsonSerializer.Serialize(paramsJson, options);

        var command = new Command
        {
            Id = Guid.NewGuid().ToString(),
            Type = (int) CommandType.SetPermissions,
            IsEncrypted = false,
            Parameters = json
        };

        var size = Marshal.SizeOf(command);
        var ptr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(command, ptr, false);

        // Marshal the unmanaged memory to the struct
        var commandNew = Marshal.PtrToStructure<Command>(ptr);

        var paramsJsonNew = JsonSerializer.Deserialize<ParamsJson>(commandNew.Parameters, options);

        _testOutputHelper.WriteLine($"{commandNew.Id + " - " + (CommandType) commandNew.Type + " - " + commandNew.Parameters}");

        Assert.Equal(command.Parameters, commandNew.Parameters);
        Assert.Equal(command.Id, commandNew.Id);
        Assert.Equal(command.Type, commandNew.Type);

        Assert.NotNull(paramsJsonNew?.TrustedApps);
        Assert.Equal(paramsJson.TrustedApps[0].Hash, paramsJsonNew.TrustedApps[0].Hash);
    }
}