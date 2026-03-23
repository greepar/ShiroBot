using Tomlyn;
using CH = QBotSharp.Core.ConsoleHelper;

namespace QBotSharp.Utils;

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

public class ConfigManager
{
    private readonly string _coreConfigPath;

    public ConfigManager(string? coreConfigPath = null)
    {
        _coreConfigPath = string.IsNullOrWhiteSpace(coreConfigPath)
            ? Path.Combine(AppContext.BaseDirectory, "config.toml")
            : Path.GetFullPath(coreConfigPath);
    }
    
    public async Task<CoreConfig?> LoadCoreConfig()
    {
        try
        {
            var config = new CoreConfig();
            if (!File.Exists(_coreConfigPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_coreConfigPath)!);
                CH.Warning("未找到配置文件，已生成默认配置文件 config.toml。");
                var tomlString =
                    "protocol = \"\"" + Environment.NewLine +
                    "enable_log = true" + Environment.NewLine +
                    "disable_console_input = false" + Environment.NewLine +
                    Environment.NewLine +
                    "[plugin_routes.default]" + Environment.NewLine +
                    "mode = \"blacklist\"" + Environment.NewLine +
                    "groups = []" + Environment.NewLine +
                    Environment.NewLine +
                    "[plugin_routes.plugins.DemoPlugin]" + Environment.NewLine +
                    "mode = \"whitelist\"" + Environment.NewLine +
                    "groups = [622603336, 742274811]" + Environment.NewLine;
                await File.WriteAllTextAsync(_coreConfigPath, tomlString);
                return config;
            }
            var toml = await File.ReadAllTextAsync(_coreConfigPath);
            config = TomlSerializer.Deserialize<CoreConfig>(toml);
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
                var tomlString = TomlSerializer.Serialize(newConfig);
                File.WriteAllText(configPath, tomlString);
                return newConfig;
            }
            var toml = File.ReadAllText(configPath);
            var config = TomlSerializer.Deserialize<T>(toml);
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
                var tomlString = TomlSerializer.Serialize(newConfig);
                File.WriteAllText(configPath, tomlString);
                return newConfig;
            }

            var toml = File.ReadAllText(configPath);
            var config = TomlSerializer.Deserialize<T>(toml);
            return config;
        }
        catch (Exception ex)
        {
            throw new Exception($"加载适配器 {adapterDirectory} 配置时出错: {ex.Message}", ex);
        }
    }

    public void SavePluginConfig<T>(string pluginDirectory, T config) where T : class
    {
        SaveToml(Path.Combine(pluginDirectory, "config.toml"), config);
    }

    public void SaveAdapterConfig<T>(string adapterDirectory, T config) where T : class
    {
        SaveToml(Path.Combine(adapterDirectory, "config.toml"), config);
    }

    private static void SaveToml<T>(string configPath, T config) where T : class
    {
        Directory.CreateDirectory(Path.GetDirectoryName(configPath)!);
        var tomlString = TomlSerializer.Serialize(config);
        File.WriteAllText(configPath, tomlString);
    }
}
