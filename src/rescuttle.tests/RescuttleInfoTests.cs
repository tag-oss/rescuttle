namespace Rescuttle.Tests;

public class RescuttleInfoTests
{
    [Fact]
    public void RescuttleInfoNoExceptions()
    {
        var expectedVersion = Environment.GetEnvironmentVariable("VERSION") ?? "1.0.0.0";
        var info = new RescuttleInfo();
        Assert.NotNull(info); 
        // We should get the compiled architecture, which unless in single binary mode (which we aren't for dotnet test)
        // so the expected output is MSIL (MS Intermediary Language)
        Assert.Equal("MSIL", info.Architecture);
        // We should get the compiled Assembly Version, but in dotnet test this will be a defaulted 1.0.0.0
        Assert.StartsWith(expectedVersion, info.Version);
    }
}