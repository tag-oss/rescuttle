using Rescuttle;
using Rescuttle.Config;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Rescuttle.Tests")]

var info = new RescuttleInfo();
Logger.Log($"version v{info.Version}, arch {info.Architecture}");

var config = new ConfigBuilder().BuildUsingDefaults();

// Startup phase
if (config.StartupEndpoints.Any())
{
    Logger.Log("entering startup polling phase");
    var startupResult = await PollController.PollAllAsync(config.StartupEndpoints);
    if (!startupResult) Environment.Exit(1);
}

// App Phase
int? childExitCode = null;
if (args.Any())
{
    Logger.Log("entering child application phase");
    var childProc = args.First();
    var childArgs = args.Skip(1).ToArray();
    var child = new ChildController(childProc, childArgs, Environment.CurrentDirectory);
    childExitCode = await child.RunAsync();
}

// Shutdown Phase
if (config.ShutdownEndpoints.Any())
{
    Logger.Log("entering shutdown polling phase");
    var shutdownResult = await PollController.PollAllAsync(config.ShutdownEndpoints);
    if (!shutdownResult) Environment.Exit(1);
}

// Exit
if (childExitCode.HasValue)
{
    Logger.Log($"exiting, passing child exit code of {childExitCode}");
    Environment.Exit(childExitCode.Value);
}

Logger.Log($"exiting, exit code 0 because no child process was given");
Environment.Exit(0);
