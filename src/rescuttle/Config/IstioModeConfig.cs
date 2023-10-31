namespace Rescuttle.Config;

public class IstioModeConfig : ModeConfig
{
    public bool StartupEnabled { get; set; } = true;
    public int StartupMaxRetries { get; set; } = 5;
    public FailureAction OnStartupFailure { get; set; } = FailureAction.Ignore;
    
    public bool ShutdownEnabled { get; set; } = true;
    public int ShutdownMaxRetries { get; set; } = 5;
    public FailureAction OnShutdownFailure { get; set; } = FailureAction.Ignore;


    public override string GetName() => "Istio";

    public override StartupEndpointConfig[] GetAdditionalStartupEndpoints()
    {
        if (!StartupEnabled) return Array.Empty<StartupEndpointConfig>();
        
        return new []
        {
            new StartupEndpointConfig()
            {
                Name = "Istio Startup",
                Uri = new Uri("http://127.0.0.1:15020/healthz/ready"),
                OnFailure = OnStartupFailure,
                MaxRetries = StartupMaxRetries,
                Source = EndpointConfigSource.IstioMode
            }
        };
    }

    public override ShutdownEndpointConfig[] GetAdditionalShutdownEndpoints()
    {
        if (!ShutdownEnabled) return Array.Empty<ShutdownEndpointConfig>();

        return new[]
        {
            new ShutdownEndpointConfig()
            {
                Name = "Istio Shutdown",
                Uri = new Uri("http://127.0.0.1:15020/quitquitquit"),
                OnFailure = OnShutdownFailure,
                MaxRetries = ShutdownMaxRetries,
                Source = EndpointConfigSource.IstioMode
            }
        };
    }
}