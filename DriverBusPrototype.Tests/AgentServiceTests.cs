using CrossTech.DSS.Packages.Core.Models.DTO.Event;
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
    public async Task PostEventsTest(EventDto[] events)
    {
        IAgentService agentService = new AgentService();
        var sendEvents = await agentService.SendEventsBatch(events);

        //Assert.True(sendEvents);
    }

    public static IEnumerable<object[]> Events
    {
        get
        {
            yield return new object[]
            {
                new[]
                {
                    new EventDto
                    {
                        DocGuid = Guid.NewGuid(),
                        DocAuthor = "test",
                        DocType = "??",
                        EventType = eEventType.OpenDocEvent,
                        FileName = "test.docx",
                        FilePath = "c:\\",
                        // Id = todo где поле для Id?
                    },
                    new EventDto()
                }
            };
        }
    }
}