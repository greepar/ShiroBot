using ShiroBot.Core;
using ShiroBot.SDK.Config;
namespace ShiroBot.Hosting.Context;

internal sealed class ConfigContext : IConfigContext
{
    private readonly ConfigManager _configManager = new();
    private readonly string _displayName;

    public string ConfigPath { get; }

    private ConfigContext(string configPath, string displayName)
    {
        ConfigPath = Path.GetFullPath(configPath);
        _displayName = displayName;
    }

    private sealed class NullConfigContext : IConfigContext
    {
        public string ConfigPath => string.Empty;
        public T Load<T>() where T : class, new() => new T();
        public void Save<T>(T config) where T : class { }
    }

    public static IConfigContext NullConfig()
    {
        return new NullConfigContext();
    }

    public static IConfigContext ForAdapter(string adapterConfigPath)
    {
        return new ConfigContext(adapterConfigPath, "适配器");
    }

    public static IConfigContext ForPlugin(string pluginConfigPath)
    {
        
        return new ConfigContext(pluginConfigPath, "插件");
    }

    public T Load<T>() where T : class, new()
    {
        return _configManager.LoadConfig<T>(ConfigPath, $"{_displayName}") ?? new T();
    }

    public void Save<T>(T config) where T : class
    {
        _configManager.SaveConfig(ConfigPath, config);
    }

    public IDisposable Watch<T>(Action<T> onChanged, int debounceMs = 500) where T : class, new()
    {
        var directory = Path.GetDirectoryName(ConfigPath)
                        ?? throw new InvalidOperationException($"无法解析配置文件目录: {ConfigPath}");
        Directory.CreateDirectory(directory);

        var watcher = new FileSystemWatcher(directory, Path.GetFileName(ConfigPath))
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime,
            EnableRaisingEvents = true
        };

        var timer = new Timer(_ => Reload(), null, Timeout.Infinite, Timeout.Infinite);

        RenamedEventHandler renamedHandler = (_, _) => timer.Change(Math.Max(50, debounceMs), Timeout.Infinite);
        watcher.Changed += ScheduleReload;
        watcher.Created += ScheduleReload;
        watcher.Renamed += renamedHandler;

        return new ConfigWatchSubscription(
            watcher,
            timer,
            () =>
            {
                watcher.Changed -= ScheduleReload;
                watcher.Created -= ScheduleReload;
                watcher.Renamed -= renamedHandler;
            });

        void ScheduleReload(object? _, FileSystemEventArgs __)
        {
            timer.Change(Math.Max(50, debounceMs), Timeout.Infinite);
        }

        void Reload()
        {
            try
            {
                onChanged(Load<T>());
            }
            catch (Exception ex)
            {
                ConsoleHelper.Error($"{_displayName}配置热重载失败: {ConfigPath} - {ex.Message}");
            }
        }
    }
}
