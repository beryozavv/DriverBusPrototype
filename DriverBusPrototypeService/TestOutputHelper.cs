using Xunit.Abstractions;

namespace DriverBusPrototypeService;

public class TestOutputHelper : ITestOutputHelper // todo временное решение
{
    private readonly ILogger<TestOutputHelper> _logger;

    public TestOutputHelper(ILogger<TestOutputHelper> logger)
    {
        _logger = logger;
    }
    
    public void WriteLine(string message)
    {
        _logger.LogInformation(message);
    }

    public void WriteLine(string format, params object[] args)
    {
        _logger.LogInformation(format, args);
    }
}