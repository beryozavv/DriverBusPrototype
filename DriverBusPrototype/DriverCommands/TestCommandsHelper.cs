using System.Text.Json;
using CrossTech.DSS.Packages.Core.Models.Enums;
using DriverBusPrototype.DriverCommands.Models;

namespace DriverBusPrototype.DriverCommands;

public static class TestCommandsHelper
{
    public static Command PrepareParamsCommand()
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
            Type = CommandType.SetParams,
            Parameters = json
        };
        return command;
    }

    public static Command PreparePermissionsCommand()
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
            Type = CommandType.SetPermissions,
            Parameters = json
        };
        return command;
    }
}