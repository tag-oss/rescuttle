namespace Rescuttle;

/// <summary>
/// A very simple and lightweight "logger" to stdout/console
/// </summary>
public static class Logger
{
    const string PREFIX = "Rescuttle:";
    
    public static void Log(string message)
    {
        Console.WriteLine($"{PREFIX} {message}");
    }
}