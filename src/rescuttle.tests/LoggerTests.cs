namespace Rescuttle.Tests;

public class LoggerTests
{
    [Fact]
    public void LogsWithoutException()
    {
        Logger.Log("Hello world");
    }
}