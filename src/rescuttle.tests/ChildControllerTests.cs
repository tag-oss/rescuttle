namespace Rescuttle.Tests;

/// <summary>
/// These unit tests verify Rescuttle's ability to run child processes and capture exit codes.  They depend on the
/// environment where `dotnet test` is has certain binaries like "echo".  Any usual Unix Shell environment should
/// work.  
/// </summary>
public class ChildControllerTests
{
    [Fact]
    public async Task ChildControllerRunsEcho()
    {
        var controller = new ChildController("echo", new[] { "hello", "world" }, "");
        var exitCode = await controller.RunAsync();
        Assert.Equal(0, exitCode);
    }
    
    
    [Fact]
    public async Task ChildControllerHasCorrectBadExitCode()
    {
        var expectedCode = 254;
        var controller = new ChildController("/bin/bash", new[] { $"-c \"exit {expectedCode}\""}, "");
        var exitCode = await controller.RunAsync();
        Assert.Equal(expectedCode, exitCode);
    }
}