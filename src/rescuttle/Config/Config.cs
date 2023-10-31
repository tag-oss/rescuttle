namespace Rescuttle.Config;


/// <summary>
/// Scuttle's configuration
/// </summary>
public class Config
{
    public List<StartupEndpointConfig> StartupEndpoints { get; set; } = new List<StartupEndpointConfig>();
    public List<ShutdownEndpointConfig> ShutdownEndpoints { get; set; } = new List<ShutdownEndpointConfig>();
    public IstioModeConfig Istio { get; set; } = new IstioModeConfig();
}