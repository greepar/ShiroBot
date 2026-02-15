using GeneratedTomlTemplates;
using Tomlyn;
using CH = QBotSharp.Utils.ConsoleHelper;

namespace QBotSharp.Utils;

//总配置类
[TomlModel]
public class CoreConfig
{
    public string Protocol { get; set; } = "milky";
    public bool EnableLog { get; set; } = true;
}

public class ConfigManager
{
    private readonly string _currentPath = AppContext.BaseDirectory;
    private readonly string _coreConfigPath = Path.Combine(AppContext.BaseDirectory, "config.toml");
    
    public async Task<CoreConfig> LoadCoreConfig()
    {
        try
        {
            var config = new CoreConfig();
            if (!File.Exists(_coreConfigPath))
            {
                CH.Warning("未找到配置文件，已生成默认配置文件 config.toml。");
                var tomlString = Toml.FromModel(new CoreConfig());
                // var tomlString = CoreConfig_TomlTemplate.Content;
                await File.WriteAllTextAsync(_coreConfigPath, tomlString);
                return config;
            }
            var toml = await File.ReadAllTextAsync(_coreConfigPath);
            config = Toml.ToModel<CoreConfig>(toml);
            return config;
        }
        catch (Exception ex)
        {
            throw new Exception($"加载配置时出错:{ex.Message}");
        }
    }

    public T LoadPluginConfig<T>(string pluginName) where T : class, new()
    {
        var configPath = Path.Combine(_currentPath, "plugins", pluginName, "config.toml");
        try
        {
            if (!File.Exists(configPath))
            {
                var newConfig = Activator.CreateInstance<T>();
                CH.Warning($"未找到插件 {pluginName} 的配置文件，已生成默认配置文件 config.toml。");
                var tomlString = Toml.FromModel(newConfig);
                File.WriteAllText(configPath, tomlString);
                return newConfig;
            }
            var toml = File.ReadAllText(configPath);
            var config = Toml.ToModel<T>(toml);
            return config;
        }
        catch
        {
            throw new Exception($"加载插件 {pluginName} 配置时出错");
        }
    }
}