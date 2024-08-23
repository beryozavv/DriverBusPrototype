using CrossTech.DSS.Packages.Core.Models.Enums;
using DriverBusPrototype.Agent;
using Xunit;
using Xunit.Abstractions;

namespace DriverBusPrototype.Tests;

public class AgentServiceTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public AgentServiceTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory, MemberData(nameof(Guids))]
    public async Task GetDocEncryptionKeyTest(Guid docGuid)
    {
        IAgentService agentService = new AgentService();


        var encryptionKey = await agentService.GetEncryptionKey(docGuid);

        _testOutputHelper.WriteLine($"Doc = {docGuid} Key = {encryptionKey}");

        Assert.NotNull(encryptionKey);
        Assert.NotEmpty(encryptionKey);
    }

    public static IEnumerable<object[]> Guids
    {
        get
        {
            yield return new object[] { Guid.Parse("0FAE4532-1274-47EA-B980-ECA1CB5B9F70") };
            yield return new object[] { Guid.Parse("6F69E2EF-1ECE-45CD-A2E7-1F720FC50B12") };
        }
    }

    [Theory, MemberData(nameof(Events))]
    public async Task PostEventsTest(DriverEventDto[] events)
    {
        IAgentService agentService = new AgentService();
        var sendEventsId = await agentService.SendEventsBatch(events);
        _testOutputHelper.WriteLine($"readMaxId={sendEventsId}");
        
        var maxId = events.Max(e => e.DriverId);
        Assert.Equal(maxId, sendEventsId);
        
        var sendEmptyEventsId = await agentService.SendEventsBatch(Array.Empty<DriverEventDto>());
        Assert.Equal(sendEventsId, sendEmptyEventsId);
    }

    public static IEnumerable<object[]> Events
    {
        get
        {
            yield return new object[]
            {
                new[]
                {
                    new DriverEventDto
                    {
                        DriverId = 120,
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
                        DriverId = 125,
                        DocGuid = Guid.NewGuid(),
                        DocAuthor = "test",
                        DocType = "??",
                        EventType = eEventType.OpenDocEvent,
                        EventDateTimeUtc = DateTime.UtcNow,
                        FileName = "test.docx",
                        FilePath = "c:\\test.docx",
                        UserId = "123testSid789",
                        MarkerGuid = Guid.NewGuid()
                    }
                }
            };
        }
    }
}