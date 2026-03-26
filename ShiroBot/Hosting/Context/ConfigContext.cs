using ShiroBot.Core;
using ShiroBot.SDK.Config;
using ShiroBot.SDK.Plugin;
using CH = ShiroBot.Core.ConsoleHelper;

namespace ShiroBot.Hosting.Context;

internal sealed class ConfigContext : IConfigContext
{
    private readonly ConfigManager _configManager = new();
    private readonly string _configPath;
    private readonly string _displayName;

    public string ConfigPath => _configPath;

    private ConfigContext(string configPath, string displayName)
    {
        _configPath = Path.GetFullPath(configPath);
        _displayName = displayName;
    }

    public static IConfigContext ForAdapter(string adapterConfigPath)
    {
        return new ConfigContext(adapterConfigPath, "适配器");
    }

    public static IConfigContext ForPlugin(string pluginDirectory)
    {
        return new ConfigContext(Path.Combine(pluginDirectory, "config.toml"), "插件");
    }

    public T Load<T>() where T : class, new()
    {
        return _configManager.LoadConfig<T>(_configPath, $"{_displayName}") ?? new T();
    }

    public void Save<T>(T config) where T : class
    {
        _configManager.SaveConfig(_configPath, config);
    }

    public IDisposable Watch<T>(Action<T> onChanged, int debounceMs = 500) where T : class, new()
    {
        var directory = Path.GetDirectoryName(_configPath)
                        ?? throw new InvalidOperationException($"无法解析配置文件目录: {_configPath}");
        Directory.CreateDirectory(directory);

        Timer? timer = null;
        var watcher = new FileSystemWatcher(directory, Path.GetFileName(_configPath))
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime,
            EnableRaisingEvents = true
        };

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

        timer = new Timer(_ => Reload(), null, Timeout.Infinite, Timeout.Infinite);

        void ScheduleReload(object? _, FileSystemEventArgs __)
        {
            timer.Change(Math.Max(50, debounceMs), Timeout.Infinite);
        }

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
    }
}
