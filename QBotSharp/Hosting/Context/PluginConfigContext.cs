using QBotSharp.SDK.Plugin;
using QBotSharp.Utils;
using CH = QBotSharp.Core.ConsoleHelper;

namespace QBotSharp.Hosting.Context;

internal sealed class PluginConfigContext(string pluginDirectory) : IPluginConfigContext
{
    private readonly ConfigManager _configManager = new();
    public string ConfigPath => Path.Combine(pluginDirectory, "config.toml");

    public T Load<T>() where T : class, new()
    {
        return _configManager.LoadPluginConfig<T>(pluginDirectory) ?? new T();
    }

    public void Save<T>(T config) where T : class
    {
        _configManager.SavePluginConfig(pluginDirectory, config);
    }

    public IDisposable Watch<T>(Action<T> onChanged, int debounceMs = 500) where T : class, new()
    {
        Directory.CreateDirectory(pluginDirectory);

        Timer? timer = null;
        var watcher = new FileSystemWatcher(pluginDirectory, Path.GetFileName(ConfigPath))
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime,
            EnableRaisingEvents = true
        };

        void Reload()
        {
            try
            {
                var config = Load<T>();
                onChanged(config);
            }
            catch (Exception ex)
            {
                CH.Error($"插件配置热重载失败: {ConfigPath} - {ex.Message}");
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
