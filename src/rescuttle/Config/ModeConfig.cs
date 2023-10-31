namespace Rescuttle.Config;

/// <summary>
/// Abstract base mode, should be implemented by any modes Rescuttle supports (e.g. Istio)
/// </summary>
public abstract class ModeConfig
{
    public bool Enabled { get; set; } = false;
    public abstract string GetName();
    public abstract StartupEndpointConfig[] GetAdditionalStartupEndpoints();
    public abstract ShutdownEndpointConfig[] GetAdditionalShutdownEndpoints();
}