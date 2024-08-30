using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace DriverBusPrototype.Tests.Mocks;

public class LoggerMock<TCategoryName> : ILogger<TCategoryName>
{
    private readonly ITestOutputHelper _testOutputHelper;

    public LoggerMock(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }
    
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var message = formatter.Invoke(state, exception);
        _testOutputHelper.WriteLine(message);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }
}