using Rescuttle.Config;

namespace Rescuttle.Tests;

/// <summary>
/// Tests PollController, which makes all our HTTP calls and handles responses based on configs.
///
/// These are arguably integration tests because they make calls to HttpBin.org to verify functionality.  They will
/// fail without network access!
/// </summary>
public class PollControllerTests
{
    /// <summary>
    /// Poll controller should not throw exceptions when given bad config.  It should give a controlled response
    /// with a result enum and Exception
    /// </summary>
    [Fact]
    public async Task PollControllerFailures()
    {
        var poller = new PollController(new StartupEndpointConfig());
        var poll = await poller.PollAsync();
        Assert.False(poll.Result == PollResult.Success);
        Assert.True(poll.Result == PollResult.FailWithIgnore);
        Assert.NotNull(poll.Exception);
    }
    
    /// <summary>
    /// A response of 4xx type should be considered a failure
    /// </summary>
    [Fact]
    public async Task PollControllerFailures4Xx()
    {
        var poller = new PollController(new StartupEndpointConfig
        {
            Method = StartupEndpointHttpMethod.Post,
            Uri = new Uri("https://httpbin.org/status/400"),
            OnFailure = FailureAction.Fail
        });
        var poll = await poller.PollAsync();
        // Should match above OnFailure
        Assert.True(poll.Result == PollResult.FailWithFail);
        Assert.NotNull(poll.Exception);
    }
    
    /// <summary>
    /// A response of 5xx type should be considered a failure
    /// </summary>
    [Fact]
    public async Task PollControllerFailures5Xx()
    {
        var poller = new PollController(new StartupEndpointConfig
        {
            Method = StartupEndpointHttpMethod.Post,
            Uri = new Uri("https://httpbin.org/status/500"),
            OnFailure = FailureAction.Ignore
        });
        var poll = await poller.PollAsync();
        // Should match above OnFailure
        Assert.True(poll.Result == PollResult.FailWithIgnore);
        Assert.NotNull(poll.Exception);
    }
    
    
    /// <summary>
    /// A response of 5xx type should be considered a failure
    /// </summary>
    [Fact]
    public async Task PollControllerSuccess()
    {
        var poller = new PollController(new StartupEndpointConfig
        {
            Method = StartupEndpointHttpMethod.Post,
            Uri = new Uri("https://httpbin.org/status/200"),
            OnFailure = FailureAction.Ignore
        });
        var poll = await poller.PollAsync();
        Assert.True(poll.Result == PollResult.Success);
        Assert.Null(poll.Exception);
    }
    
    /// <summary>
    /// When using the static PollAllAsync with some successes and some failures with FailureActionIgnore,
    /// we should still get a successful response (because failures are ignored)
    /// </summary>
    [Fact]
    public async Task PollControllerAllOneFailureIgnored()
    {
        var pollAll = await PollController.PollAllAsync(new[]
        {
            new StartupEndpointConfig
            {
                Method = StartupEndpointHttpMethod.Post,
                Uri = new Uri("https://httpbin.org/status/200"),
                OnFailure = FailureAction.Fail
            },
            new StartupEndpointConfig
            {
                Method = StartupEndpointHttpMethod.Post,
                Uri = new Uri("https://httpbin.org/status/500"),
                OnFailure = FailureAction.Ignore
            }
        });

        Assert.True(pollAll);
    }
    
    /// <summary>
    /// When using the static PollAllAsync with some successes and some failures with FailureActionFail,
    /// we should NOT get a successful response (because failures are NOT ignored)
    /// </summary>
    [Fact]
    public async Task PollControllerAllOneFailureNotIgnored()
    {
        var pollAll = await PollController.PollAllAsync(new[]
        {
            new StartupEndpointConfig
            {
                Method = StartupEndpointHttpMethod.Post,
                Uri = new Uri("https://httpbin.org/status/200"),
                OnFailure = FailureAction.Fail
            },
            new StartupEndpointConfig
            {
                Method = StartupEndpointHttpMethod.Post,
                Uri = new Uri("https://httpbin.org/status/500"),
                OnFailure = FailureAction.Fail
            }
        });

        Assert.False(pollAll);
    }
    
    
    /// <summary>
    /// When using the static PollAllAsync with all successful response codes we should get
    /// a positive/true result back
    /// </summary>
    [Fact]
    public async Task PollControllerAllNoFailures()
    {
        var pollAll = await PollController.PollAllAsync(new[]
        {
            new StartupEndpointConfig
            {
                Method = StartupEndpointHttpMethod.Post,
                Uri = new Uri("https://httpbin.org/status/200"),
                OnFailure = FailureAction.Fail
            },
            new StartupEndpointConfig
            {
                Method = StartupEndpointHttpMethod.Post,
                Uri = new Uri("https://httpbin.org/status/200"),
                OnFailure = FailureAction.Fail
            }
        });

        Assert.True(pollAll);
    }
}