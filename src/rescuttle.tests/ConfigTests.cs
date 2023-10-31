using Rescuttle.Config;

namespace Rescuttle.Tests;

public class ConfigTests
{
    internal static Config.Config GetConfig()
    {
        return new ConfigBuilder()
            .WithUserInput(jsonFile: "tests.rescuttle.everything.json", fileOptional: false)
            .WithModes()
            .Build();
    }
    
    [Fact]
    public void StartupEndpointsLoaded()
    {
        var config = GetConfig();
        Assert.NotEmpty(config.StartupEndpoints);
        var first = config.StartupEndpoints.First();
        Assert.Equal(StartupEndpointHttpMethod.Get, first.Method);
        Assert.Equal("A Custom Startup Poller", first.Name);
        Assert.Equal(new Uri("https://httpbin.org/get"), first.Uri);
        Assert.Equal(FailureAction.Ignore, first.OnFailure);
        Assert.Equal(99, first.MaxRetries);
    }
    
    [Fact]
    public void ShutdownEndpointsLoaded()
    {
        var config = GetConfig();
        Assert.NotEmpty(config.ShutdownEndpoints);
        var first = config.ShutdownEndpoints.First();
        Assert.Equal(ShutdownEndpointHttpMethod.Post, first.Method);
        Assert.Equal("A Custom Shutdown Webhook", first.Name);
        Assert.Equal(new Uri("https://httpbin.org/post"), first.Uri);
        Assert.Equal(FailureAction.Fail, first.OnFailure);
        Assert.Equal(-1, first.MaxRetries);
    }
    
        
    [Fact]
    public void IstioShutdownEndpointsLoaded()
    {
        var config = GetConfig();
        Assert.NotEmpty(config.ShutdownEndpoints);
        var istioShutdown = config.ShutdownEndpoints[1];
        Assert.Equal(EndpointConfigSource.IstioMode, istioShutdown.Source);
        Assert.Equal(ShutdownEndpointHttpMethod.Post, istioShutdown.Method);
        Assert.Equal("Istio Shutdown", istioShutdown.Name);
        Assert.Equal(new Uri("http://127.0.0.1:15020/quitquitquit"), istioShutdown.Uri);
        Assert.Equal(FailureAction.Ignore, istioShutdown.OnFailure);
        Assert.Equal(5, istioShutdown.MaxRetries);
    }
        
    [Fact]
    public void IstioStartupEndpointsLoaded()
    {
        var config = GetConfig();
        Assert.NotEmpty(config.StartupEndpoints);
        var istioShutdown = config.StartupEndpoints.First(e => e.Name == "Istio Startup");
        Assert.Equal(EndpointConfigSource.IstioMode, istioShutdown.Source);
        Assert.Equal(StartupEndpointHttpMethod.Get, istioShutdown.Method);
        Assert.Equal(new Uri("http://127.0.0.1:15020/healthz/ready"), istioShutdown.Uri);
        Assert.Equal(FailureAction.Fail, istioShutdown.OnFailure);
        Assert.Equal(5, istioShutdown.MaxRetries);
    }  
    
    [Fact]
    public void IstioEnabled()
    {
        var config = GetConfig();
        Assert.NotNull(config);
        Assert.NotNull(config.Istio);
        Assert.True(config.Istio.Enabled);
    }
}