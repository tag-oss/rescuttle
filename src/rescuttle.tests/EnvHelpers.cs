namespace Rescuttle.Tests;

/// <summary>
/// Disposable env var wrapper to ensure variables are cleaned up automatically after each test
/// </summary>
public class EnvHelper : IDisposable
{
    private readonly HashSet<string> _cleanupKeys = new HashSet<string>();
    
    public void Set(string key, string value)
    {
        _cleanupKeys.Add(key);
        Environment.SetEnvironmentVariable(key, value);
    }

    public void Dispose()
    {
        foreach (var key in _cleanupKeys)
        {
            Environment.SetEnvironmentVariable(key, null);
        }
    }
}