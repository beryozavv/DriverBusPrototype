using System.Text.Json;
using CrossTech.DSS.Packages.Core.Models.Enums;
using DriverBusPrototype.Models;
using Xunit;

namespace DriverBusPrototype.Tests;

public class SerializationTests
{
    [Fact]
    public void GuidDeserialize()
    {
        var command = new Command
        {
            Id = Guid.NewGuid(),
            Type = CommandType.GetEncryptionKey,
            IsEncrypted = false,
            Parameters = JsonSerializer.Serialize(Guid.NewGuid())
        };

        var serialize = JsonSerializer.Serialize(command, BusJsonOptions.GetOptions());
        
        var commandD = JsonSerializer.Deserialize<Command>(serialize, BusJsonOptions.GetOptions());

        var deserialize = JsonSerializer.Deserialize<Guid>(commandD.Parameters);

        var str =
            "{\"Id\":\"977c16ac-995a-4c9e-b451-b38a578059b4\",\"Type\":\"GetEncryptionKey\",\"IsEncrypted\":false,\"Parameters\":\"\\u0022ebebbb69-f9b3-4246-a937-f3bfe0f8f816\\u0022\"}";

        var deserialize1 = JsonSerializer.Deserialize<Command>(str, BusJsonOptions.GetOptions());
    }
    
    [Fact]
    public void EventsDeserialize()
    {
        var command = new Command
        {
            Id = Guid.NewGuid(),
            Type = CommandType.SendEventsBatch,
            IsEncrypted = false,
            Parameters = JsonSerializer.Serialize(GetEvents())
        };

        var serialize = JsonSerializer.Serialize(command, BusJsonOptions.GetOptions());
        
        var commandD = JsonSerializer.Deserialize<Command>(serialize, BusJsonOptions.GetOptions());

        var deserialize = JsonSerializer.Deserialize<DriverEventDto[]>(commandD.Parameters);

        var str =
            "{\"Id\":\"109c933d-7457-4434-a24a-f258b7005432\",\"Type\":\"SendEventsBatch\",\"IsEncrypted\":false,\"Parameters\":\"[{\\u0022DriverEventId\\u0022:120,\\u0022EventType\\u0022:4,\\u0022EventDateTimeUtc\\u0022:\\u00222024-09-06T13:01:31.0899814Z\\u0022,\\u0022DocGuid\\u0022:\\u00227a09cf61-2245-4727-b59b-af8f662f18d8\\u0022,\\u0022ParentGuid\\u0022:null,\\u0022DocType\\u0022:\\u0022??\\u0022,\\u0022DocAuthor\\u0022:\\u0022test\\u0022,\\u0022MarkerGuid\\u0022:\\u0022f5dfeb97-b085-4f47-9dd5-4515402e6cb8\\u0022,\\u0022EncryptionPolicyId\\u0022:null,\\u0022FileName\\u0022:\\u0022test.docx\\u0022,\\u0022FilePath\\u0022:\\u0022c:\\\\\\\\test.docx\\u0022,\\u0022ParentFileName\\u0022:null,\\u0022ParentFilePath\\u0022:null,\\u0022UserId\\u0022:\\u0022123testSid789\\u0022,\\u0022UserName\\u0022:null},{\\u0022DriverEventId\\u0022:125,\\u0022EventType\\u0022:6,\\u0022EventDateTimeUtc\\u0022:\\u00222024-09-06T13:01:31.0901452Z\\u0022,\\u0022DocGuid\\u0022:\\u00227fe60b28-b868-4a42-aa57-7d7d5214fa29\\u0022,\\u0022ParentGuid\\u0022:null,\\u0022DocType\\u0022:\\u0022??\\u0022,\\u0022DocAuthor\\u0022:\\u0022test\\u0022,\\u0022MarkerGuid\\u0022:\\u0022da3c5b9d-1dcd-4b7c-905f-e9ef6929f998\\u0022,\\u0022EncryptionPolicyId\\u0022:null,\\u0022FileName\\u0022:\\u0022test2.docx\\u0022,\\u0022FilePath\\u0022:\\u0022c:\\\\\\\\test2.docx\\u0022,\\u0022ParentFileName\\u0022:null,\\u0022ParentFilePath\\u0022:null,\\u0022UserId\\u0022:\\u0022456123testSid789\\u0022,\\u0022UserName\\u0022:null}]\"}";

    }

    private DriverEventDto[] GetEvents()
    {
        return new[]
        {
            new DriverEventDto
            {
                DriverEventId = 120,
                DocGuid = Guid.NewGuid(),
                DocAuthor = "test",
                DocType = "??",
                EventType = eEventType.OpenDocEvent,
                EventDateTimeUtc = DateTime.UtcNow,
                FileName = "test.docx",
                FilePath = "c:\\test.docx",
                UserId = "123testSid789",
                MarkerGuid = Guid.NewGuid()
            },
            new DriverEventDto
            {
                DriverEventId = 125,
                DocGuid = Guid.NewGuid(),
                DocAuthor = "test",
                DocType = "??",
                EventType = eEventType.SaveDocEvent,
                EventDateTimeUtc = DateTime.UtcNow,
                FileName = "test2.docx",
                FilePath = "c:\\test2.docx",
                UserId = "456123testSid789",
                MarkerGuid = Guid.NewGuid()
            }
        };
    }
}