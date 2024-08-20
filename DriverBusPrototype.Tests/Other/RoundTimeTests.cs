using DriverBusPrototype.AgentService.Auth;
using Xunit;
using Xunit.Abstractions;

namespace DriverBusPrototype.Tests.Other;

public class RoundTimeTests
{
    private readonly ITestOutputHelper _outputHelper;

    public RoundTimeTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    [Fact]
    public void RoundTimeLoopTest()
    {
        for (int i = 0; i < 20; i++)
        {
            var dateTime = DateTime.Now;

            var rounded = CommonHelper.RoundToNearestInterval(dateTime, TimeSpan.FromSeconds(10));

            _outputHelper.WriteLine($"{dateTime.ToString("O")} Rounded value = {rounded}, rounded dateTime = {new DateTime(rounded, dateTime.Kind).ToString("O")}");

            Thread.Sleep(TimeSpan.FromMilliseconds(1000));
        }
    }
}