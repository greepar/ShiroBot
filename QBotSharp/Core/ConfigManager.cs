using System.Text.Json;
using System.Text.Json.Serialization;
using Tomlyn;
using Tomlyn.Serialization;
using CH = QBotSharp.Core.ConsoleHelper;

namespace QBotSharp.Core;

//总配置类
public class CoreConfig
{
    public string? Protocol { get; set; }

    public bool EnableLog { get; set; } = true;

    public bool DisableConsoleInput { get; set; }

    public PluginRouteConfig PluginRoutes { get; set; } = new();
}

public class PluginRouteConfig
{
    public PluginRouteRuleConfig Default { get; set; } = new();

    public Dictionary<string, PluginRouteRuleConfig> Plugins { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public class PluginRouteRuleConfig
{
    public string Mode { get; set; } = "whitelist";

    public long[] Groups { get; set; } = [];
}

[TomlSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
public class ConfigManager(string? coreConfigPath = null)
{
    private readonly string _coreConfigPath = string.IsNullOrWhiteSpace(coreConfigPath)
        ? Path.Combine(AppContext.BaseDirectory, "config.toml")
        : Path.GetFullPath(coreConfigPath);

    private readonly TomlSerializerOptions _options = new TomlSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        WriteIndented = true,
        IndentSize = 4,
        MaxDepth = 64,
        DefaultIgnoreCondition = TomlIgnoreCondition.WhenWritingNull,
    };
    
    public async Task<CoreConfig?> LoadCoreConfig()
    {
        try
        {
            if (!File.Exists(_coreConfigPath))
            {
                var configTemplate = new CoreConfig()
                {
                    Protocol = "",
                    EnableLog = true,
                    DisableConsoleInput = false,
                    PluginRoutes = new PluginRouteConfig()
                    {
                        Default = new PluginRouteRuleConfig()
                        {
                            Mode = "blacklist",
                            Groups = []
                        },
                        Plugins = new Dictionary<string, PluginRouteRuleConfig>(StringComparer.OrdinalIgnoreCase)
                        {
                            ["DemoPlugin"] = new PluginRouteRuleConfig()
                            {
                                Mode = "whitelist",
                                Groups = [622603336, 742274811]
                            }
                        }
                    }
                };
                Directory.CreateDirectory(Path.GetDirectoryName(_coreConfigPath)!);
                CH.Warning("未找到配置文件，已生成默认配置文件 config.toml。");
                var tomlString = TomlSerializer.Serialize(configTemplate,_options);
                await File.WriteAllTextAsync(_coreConfigPath, tomlString);
                return configTemplate;
            }
            var toml = await File.ReadAllTextAsync(_coreConfigPath);
            var config = TomlSerializer.Deserialize<CoreConfig>(toml, _options);
            return config;
        }
        catch (Exception ex)
        {
            throw new Exception($"加载配置时出错:{ex.Message}");
        }
    }

    public T? LoadPluginConfig<T>(string pluginDirectory) where T : class, new()
    {
        var configPath = Path.Combine(pluginDirectory, "config.toml");
        try
        {
            Directory.CreateDirectory(pluginDirectory);
            if (!File.Exists(configPath))
            {
                var newConfig = Activator.CreateInstance<T>();
                CH.Warning($"未找到插件目录 {pluginDirectory} 的配置文件，已生成默认配置文件 config.toml。");
                var tomlString = TomlSerializer.Serialize(newConfig, _options);
                File.WriteAllText(configPath, tomlString);
                return newConfig;
            }
            var toml = File.ReadAllText(configPath);
            var config = TomlSerializer.Deserialize<T>(toml, _options);
            return config;
        }
        catch (Exception ex)
        {
            throw new Exception($"加载插件目录 {pluginDirectory} 配置时出错: {ex.Message}", ex);
        }
    }

    public T? LoadAdapterConfig<T>(string adapterDirectory) where T : class, new()
    {
        var configPath = Path.Combine(adapterDirectory, "config.toml");
        try
        {
            Directory.CreateDirectory(adapterDirectory);
            if (!File.Exists(configPath))
            {
                var newConfig = Activator.CreateInstance<T>();
                CH.Warning($"未找到适配器目录 {adapterDirectory} 的配置文件，已生成默认配置文件 config.toml。");
                var tomlString = TomlSerializer.Serialize(newConfig, _options);
                File.WriteAllText(configPath, tomlString);
                return newConfig;
            }

            var toml = File.ReadAllText(configPath);
            var config = TomlSerializer.Deserialize<T>(toml, _options);
            return config;
        }
        catch (Exception ex)
        {
            throw new Exception($"加载适配器 {adapterDirectory} 配置时出错: {ex.Message}", ex);
        }
    }

    public void SavePluginConfig<T>(string pluginDirectory, T config) where T : class
    {
        SaveToml(Path.Combine(pluginDirectory, "config.toml"), config, _options);
    }

    public void SaveAdapterConfig<T>(string adapterDirectory, T config) where T : class
    {
        SaveToml(Path.Combine(adapterDirectory, "config.toml"), config, _options);
    }

    private static void SaveToml<T>(string configPath, T config, TomlSerializerOptions options) where T : class
    {
        Directory.CreateDirectory(Path.GetDirectoryName(configPath)!);
        var tomlString = TomlSerializer.Serialize(config, options);
        File.WriteAllText(configPath, tomlString);
    }
}
