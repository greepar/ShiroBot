using System.Text.Json;
using QBotSharp.SDK;
using QBotSharp.SDK.Abstractions;
using Tomlyn;
using CH = QBotSharp.Core.ConsoleHelper;

namespace QBotSharp.Core;

//总配置类
public class CoreConfig
{
    public string Protocol { get; set; } = string.Empty;

    public bool EnableLog { get; set; } = true;

    public bool DisableConsoleInput { get; set; } = false;

    public PluginRouteConfig PluginRoutes { get; set; } = new()
    {
        Default = new PluginRouteRuleConfig
        {
            Mode = "blacklist",
            Groups = []
        }
    };
}

public class PluginRouteConfig
{
    public PluginRouteRuleConfig Default { get; set; } = new();

    public Dictionary<string, PluginRouteRuleConfig> Plugins { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public bool AllowsGroup(string pluginName, long groupId)
    {
        if (!Plugins.TryGetValue(pluginName, out var rule))
        {
            return Default.IsMatch(groupId);
        }

        return rule.IsMatch(groupId);
    }
}

public class PluginRouteRuleConfig
{
    public string Mode { get; set; } = "whitelist";

    public long[] Groups { get; set; } = [];

    public bool IsMatch(long groupId)
    {
        var contains = Groups.Contains(groupId);
        return NormalizeMode(Mode) switch
        {
            "blacklist" => !contains,
            _ => contains
        };
    }

    private static string NormalizeMode(string? mode)
    {
        return string.IsNullOrWhiteSpace(mode) ? "whitelist" : mode.ToLowerInvariant();
    }
}

public class ConfigManager(string? coreConfigPath = null)
{
    private readonly string _coreConfigPath = string.IsNullOrWhiteSpace(coreConfigPath)
        ? Path.Combine(AppContext.BaseDirectory, "config.toml")
        : Path.GetFullPath(coreConfigPath);

    private readonly TomlSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        WriteIndented = true,
        IndentSize = 4,
        MaxDepth = 64,
        DefaultIgnoreCondition = TomlIgnoreCondition.WhenWritingNull,
    };
    
    public async Task<CoreConfig> LoadCoreConfig()
    {
        try
        {
            if (!File.Exists(_coreConfigPath)) return await CreateDefaultConfig();
            var tomlString = await File.ReadAllTextAsync(_coreConfigPath);
            if (string.IsNullOrWhiteSpace(tomlString)) return await CreateDefaultConfig();
            var config = TomlSerializer.Deserialize<CoreConfig>(tomlString, _options);
            return config;
        }
        catch (Exception ex)
        {
            throw new Exception($"加载配置时出错:{ex.Message}");
        }
        
        //创建默认配置保存的方法
        async Task<CoreConfig> CreateDefaultConfig()
        {
            BotLog.Info("未找到配置文件，正在创建默认配置...");
            var defaultConfig = new CoreConfig();
            var tomlString = FormatToml(TomlSerializer.Serialize(defaultConfig, _options));
            Directory.CreateDirectory(Path.GetDirectoryName(_coreConfigPath)!);
            await File.WriteAllTextAsync(_coreConfigPath, tomlString);
            return defaultConfig;
        }
    }

    public T? LoadPluginConfig<T>(string pluginDirectory) where T : class, new()
    {
        return LoadScopedConfig<T>(pluginDirectory, "插件目录");
    }

    public T? LoadAdapterConfig<T>(string adapterDirectory) where T : class, new()
    {
        return LoadScopedConfig<T>(adapterDirectory, "适配器目录");
    }

    public void SavePluginConfig<T>(string pluginDirectory, T config) where T : class
    {
        SaveScopedConfig(pluginDirectory, config);
    }

    public void SaveAdapterConfig<T>(string adapterDirectory, T config) where T : class
    {
        SaveScopedConfig(adapterDirectory, config);
    }

    public T? LoadScopedConfig<T>(string directory, string scopeName) where T : class, new()
    {
        var configPath = Path.Combine(directory, "config.toml");
        try
        {
            Directory.CreateDirectory(directory);
            if (!File.Exists(configPath))
            {
                var newConfig = Activator.CreateInstance<T>();
                CH.Warning($"未找到{scopeName} {directory} 的配置文件，已生成默认配置文件 config.toml。");
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
            throw new Exception($"加载{scopeName} {directory} 配置时出错: {ex.Message}", ex);
        }
    }

    public void SaveScopedConfig<T>(string directory, T config) where T : class
    {
        SaveToml(Path.Combine(directory, "config.toml"), config, _options);
    }

    private static void SaveToml<T>(string configPath, T config, TomlSerializerOptions options) where T : class
    {
        Directory.CreateDirectory(Path.GetDirectoryName(configPath)!);
        var tomlString = FormatToml(TomlSerializer.Serialize(config, options));
        File.WriteAllText(configPath, tomlString);
    }

    private static string FormatToml(string toml)
    {
        var lines = toml.Replace("\r\n", "\n").Split('\n');
        var builder = new System.Text.StringBuilder();
        
        foreach (var line in lines)
        {
            var trimmed = line.TrimStart();
            var isTableHeader = trimmed.StartsWith('[') && trimmed.EndsWith(']');

            if (isTableHeader && builder.Length > 0)
            {
                var current = builder.ToString();
                if (!current.EndsWith("\n\n", StringComparison.Ordinal))
                {
                    builder.AppendLine();
                }
            }

            builder.AppendLine(line);
        }

        return builder.ToString();
    }
}
