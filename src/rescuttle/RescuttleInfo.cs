namespace Rescuttle;

/// <summary>
/// A safe way to get assembly data like version and compiled architecture
/// </summary>
public class RescuttleInfo
{
    public string Version { get; } = "Unknown";
    public string Architecture { get; } = "Unknown";
    
    public RescuttleInfo()
    {
        try
        {
            var assemblyName = typeof(Program).Assembly.GetName();
            if (assemblyName.Version != null)
                Version = assemblyName.Version.ToString();
            Architecture = assemblyName.ProcessorArchitecture.ToString();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}