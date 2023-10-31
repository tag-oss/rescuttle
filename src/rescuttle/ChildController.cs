using System.Diagnostics;

namespace Rescuttle;

public class ChildController
{
    private readonly Process _process;
    
    public ChildController(string executable, string[] args, string workDir)
    {
        _process = new Process
        {
            StartInfo = 
            {
                FileName = executable,
                Arguments =  string.Join(' ', args),
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                WorkingDirectory = workDir,
            },
        };
    }

    /// <summary>
    /// Runs the child process, returns true if exited successfully and false if it did not (non zero)
    /// </summary>
    public async Task<int> RunAsync()
    {
        Logger.Log($"Starting process, '{_process.StartInfo.FileName} {_process.StartInfo.Arguments}'");
        _process.Start();
        await _process.WaitForExitAsync();
        return _process.ExitCode;
    }
}