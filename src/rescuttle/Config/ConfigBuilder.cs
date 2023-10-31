using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Rescuttle.Config;

public class ConfigBuilder
{
    private readonly ConfigurationBuilder _configBuilder = new();
    private IConfigurationRoot? _root;
    private Config _config = new();
    
    internal ConfigBuilder WithUserInput(string jsonFile="rescuttle.json", bool fileOptional = true)
    {
        // Load file into _config ourselves due to strange behavior of IConfiguration JsonProvider when dealing with
        // typos in enum fields
        LoadJsonFile(jsonFile, fileOptional);
        
        _configBuilder.AddEnvironmentVariables(prefix: "Rescuttle__");

        _root = _configBuilder.Build();
        
        _root.Bind(_config, o =>
        {
            o.ErrorOnUnknownConfiguration = true;
        });

        return this;
    }

    private void LoadJsonFile(string jsonFile, bool fileOptional)
    {
        var exists = File.Exists(jsonFile);
        if (!exists && !fileOptional)
            throw new FileNotFoundException($"Could not find required file: {jsonFile}");
        if (!exists) return;

        var text = File.ReadAllText(jsonFile);
        var settings = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Error
        };
        var config = JsonConvert.DeserializeObject<Config>(text, settings)!;
        _config = config ?? throw new FormatException("Null config after reading json, is the formatting correct?");
    }

    public Config BuildUsingDefaults()
    {
        WithUserInput();
        WithModes();
        return Build();
    }

    public Config Build() => _config;

    internal ConfigBuilder WithModes()
    {
        if (_config.Istio.Enabled)
            AddModeEndpoints(_config.Istio);

        return this;
    }

    /// <summary>
    /// Adds any endpoints defined in the given mode and combines them with the user given configuration
    /// </summary>
    /// <param name="mode"></param>
    private void AddModeEndpoints(ModeConfig mode)
    {
        Logger.Log($"Enabling {mode.GetName()} Mode");
        _config.StartupEndpoints.AddRange(mode.GetAdditionalStartupEndpoints());
        _config.ShutdownEndpoints.AddRange(mode.GetAdditionalShutdownEndpoints());
    }
}