namespace Rescuttle.Tests;

/// <summary>
/// Ensures Env vars are correctly set/overriding json in config
/// </summary>
public class EnvConfigTests
{
    [Fact]
    public void EnsureEnvVarsInConfigOverride()
    {
        using var env = new EnvHelper();
        const string expectedUri = "https://this-is-an-env-var.com/hello-world";
        const int expectedRetires = 999;
        const string expectedName = "I am from env vars";
        // Use random casing to ensure case-insensitive
        env.Set("Rescuttle__STARTUPEndpoints__0__URI", expectedUri);
        env.Set("Rescuttle__StartupEndpoints__0__MaxRetries", expectedRetires.ToString());
        env.Set("Rescuttle__StartupEndpoints__0__NAME", expectedName);
        var config = ConfigTests.GetConfig();
        Assert.NotNull(config);
        Assert.NotEmpty(config.StartupEndpoints);
        var first = config.StartupEndpoints.First(e => e.Name == expectedName);
        Assert.NotNull(first);
        Assert.Equal(new Uri(expectedUri), first.Uri);
    }
    
    
    [Fact]
    public void EnsureEnvVarsInConfigOverrideIstioMode()
    {
        using var env = new EnvHelper();
        env.Set("Rescuttle__ISTIO__enabled", "fAlSe");
        var config = ConfigTests.GetConfig();
        Assert.NotNull(config);
        Assert.NotNull(config.Istio);
        Assert.False(config.Istio.Enabled);
    }
}