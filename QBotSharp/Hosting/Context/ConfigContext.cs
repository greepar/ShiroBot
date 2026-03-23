using QBotSharp.Core;
using QBotSharp.SDK.Config;
using QBotSharp.SDK.Plugin;
using CH = QBotSharp.Core.ConsoleHelper;

namespace QBotSharp.Hosting.Context;

internal sealed class ConfigContext : IConfigContext
{
    private readonly ConfigManager _configManager = new();
    private readonly string _directory;
    private readonly string _displayName;

    public string ConfigPath => Path.Combine(_directory, "config.toml");

    private ConfigContext(string directory, string displayName)
    {
        _directory = directory;
        _displayName = displayName;
    }

    public static IConfigContext ForAdapter(string adapterDirectory)
    {
        return new ConfigContext(adapterDirectory, "适配器");
    }

    public static IConfigContext ForPlugin(string pluginDirectory)
    {
        return new ConfigContext(pluginDirectory, "插件");
    }

    public T Load<T>() where T : class, new()
    {
        return _configManager.LoadScopedConfig<T>(_directory, $"{_displayName}目录") ?? new T();
    }

    public void Save<T>(T config) where T : class
    {
        _configManager.SaveScopedConfig(_directory, config);
    }

    public IDisposable Watch<T>(Action<T> onChanged, int debounceMs = 500) where T : class, new()
    {
        Directory.CreateDirectory(_directory);

        Timer? timer = null;
        var watcher = new FileSystemWatcher(_directory, Path.GetFileName(ConfigPath))
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
                CH.Error($"{_displayName}配置热重载失败: {ConfigPath} - {ex.Message}");
            }
        }

        timer = new Timer(_ => Reload(), null, Timeout.Infinite, Timeout.Infinite);

        void ScheduleReload(object? _, FileSystemEventArgs __)
        {
            timer.Change(Math.Max(50, debounceMs), Timeout.Infinite);
        }

        watcher.Changed += ScheduleReload;
        watcher.Created += ScheduleReload;
        watcher.Renamed += (_, _) => timer.Change(Math.Max(50, debounceMs), Timeout.Infinite);

        return new ConfigWatchSubscription(watcher, timer);
    }
}
