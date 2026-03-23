using System.CommandLine;
using System.Diagnostics;
using System.Text;
using QBotSharp.Hosting;
using QBotSharp.Hosting.Context;
using QBotSharp.SDK;
using QBotSharp.SDK.Core;
using QBotSharp.Utils;
using CH = QBotSharp.Core.ConsoleHelper;

namespace QBotSharp;

public static class Program
{
    private static readonly IReadOnlyList<CH.ConsoleCommandOption> ConsoleCommands =
    [
        new("/help", "显示帮助信息"),
        new("/path", "打开当前程序目录"),
        new("/log", "切换日志输出"),
        new("/clear", "清除控制台"),
        new("/unload", "卸载并退出"),
        new("/exit", "退出程序"),
        new("/quit", "退出程序")
    ];

    public static async Task Main(string[] args)
    {
        BotLog.SetDefault(new ConsoleLogger());

        var adapterOption = new Option<string?>("--adapter")
        {
            Description = "指定适配器 DLL 文件路径（可以是相对路径或绝对路径）"
        };

        var pluginOption = new Option<string?>("--plugin-dir")
        {
            Description = "指定插件文件夹路径（可以是相对路径或绝对路径）"
        };

        var configOption = new Option<string?>("--config", "-c")
        {
            Description = "指定配置文件路径"
        };

        var noConsoleOption = new Option<bool>("--no-console")
        {
            Description = "禁用控制台交互输入"
        };

        var rootCommand = new RootCommand("QBotSharp 主程序")
        {
            adapterOption,
            pluginOption,
            configOption,
            noConsoleOption
        };

        var parserResult = rootCommand.Parse(args);

        CH.Info("QBotSharp 启动中...");

        DllLoader<IBotAdapter>? adapterLoader = null;
        IBotAdapter? adapter = null;
        var loadedPlugins = new List<(IBotPlugin Plugin, DllLoader<IBotPlugin> Loader)>();

        try
        {
            //load from command
            var configuredConfigPath = NormalizeOptionalPath(parserResult.GetValue(configOption));
            //load from config
            var configManager = new ConfigManager(configuredConfigPath);
            var coreConfig = await configManager.LoadCoreConfig() ?? new CoreConfig();
            CH.IsEnabled = coreConfig.EnableLog;
            var routePolicy = BuildRoutePolicy(coreConfig);

            var configuredProtocol = coreConfig.Protocol?.Trim();
            var configuredAdapterPath = NormalizeOptionalPath(parserResult.GetValue(adapterOption));
            var disableConsoleFromArg = parserResult.GetValue(noConsoleOption);

            CH.Info("配置加载完成" + (string.IsNullOrWhiteSpace(configuredProtocol)
                ? "，未配置协议，将自动选择 adapters 中的适配器"
                : "，当前协议: " + configuredProtocol));

            var adapterRoot = Path.Combine(AppContext.BaseDirectory, "adapters");
            Directory.CreateDirectory(adapterRoot);

            var adapterPath = ResolveConfiguredAdapterPath(adapterRoot, configuredProtocol, configuredAdapterPath);
            if (!File.Exists(adapterPath))
            {
                CH.Warning("未找到适配器文件: " + adapterPath);
                CH.Warning("请确认 adapters 目录下存在对应的适配器文件，或在 config.toml 中配置 protocol...");
                if (CanReadInteractiveKey())
                {
                    Console.ReadKey();
                }
                return;
            }

            CH.Log("开始加载适配器: " + adapterPath);
            adapterLoader = new DllLoader<IBotAdapter>();
            adapter = adapterLoader.Load(adapterPath);

            var adapterDirectory = Path.GetDirectoryName(adapterPath) ?? AppContext.BaseDirectory;
            adapter.Config = new AdapterConfigContext(adapterDirectory);
            adapter.Logger = new ConsoleLogger($"[Adapter:{adapter.Name}]");

            var adapterMetadata = adapter.Metadata;
            CH.Log($"适配器元数据: {adapterMetadata.Name} v{adapterMetadata.Version}");

            using (BotLog.BeginScope(adapter.Logger))
            {
                await adapter.StartAsync();
            }
            HostEventLogger.Attach(adapter.Event);
            CH.Success("加载成功,适配器: " + adapter.Name);

            var pluginRoot = NormalizeOptionalPath(parserResult.GetValue(pluginOption))
                ?? Path.Combine(AppContext.BaseDirectory, "plugins");
            Directory.CreateDirectory(pluginRoot);

            CH.Info("开始加载插件...");

            var loadedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var pluginDlls = Directory.EnumerateFiles(pluginRoot, "*.dll", SearchOption.AllDirectories)
                .Where(dll => IsPluginEntryAssembly(pluginRoot, dll))
                .OrderBy(dll => dll, StringComparer.OrdinalIgnoreCase);

            foreach (var dll in pluginDlls)
            {
                try
                {
                    var loader = new DllLoader<IBotPlugin>();
                    var plugin = loader.Load(dll);
                    var pluginContext = new BotContext(
                        adapter,
                        plugin.Name,
                        Path.GetDirectoryName(dll),
                        groupId => routePolicy.AllowsGroup(plugin.Name, groupId));

                    if (!loadedNames.Add(plugin.Name))
                    {
                        CH.Warning($"插件名重复，已跳过：{plugin.Name} ({dll})");
                        loader.Unload();
                        continue;
                    }

                    var metadata = plugin.Metadata;
                    CH.Info($"插件元数据: {metadata.Name} v{metadata.Version} ");

                    using (BotLog.BeginScope(pluginContext.Logger))
                    {
                        await plugin.OnLoad(pluginContext);
                    }
                    loadedPlugins.Add((plugin, loader));
                    CH.Success($"插件加载成功: {plugin.Name} ({dll})");
                }
                catch (Exception ex)
                {
                    CH.Error($"插件加载失败: {dll} - {ex.Message}");
                }
            }

            CH.Success("已加载插件: " + string.Join(", ", loadedPlugins.Select(p => p.Plugin.Name)));

            var botHost = await BotHostBuilder
                .CreateDefault()
                .BuildAsync();
            await botHost.RunAsync();

            var hasConsole =
                Environment.UserInteractive &&
                !Console.IsInputRedirected &&
                !Console.IsOutputRedirected;
            var enableConsoleInput = hasConsole && !disableConsoleFromArg && !coreConfig.DisableConsoleInput;

            if (!enableConsoleInput)
            {
                var reasons = new List<string>();

                if (!hasConsole)
                {
                    reasons.Add("检测到非交互终端");
                }

                if (disableConsoleFromArg)
                {
                    reasons.Add("命令行参数 --no-console 已启用");
                }

                if (coreConfig.DisableConsoleInput)
                {
                    reasons.Add("配置项 disable_console_input = true");
                }

                CH.Info($"已禁用控制台命令输入: {string.Join("，", reasons)}");
            }

            if (enableConsoleInput)
            {
                while (true)
                {
                    var input = CH.ReadPrompt("> ", ConsoleCommands);
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        continue;
                    }

                    if (CH.IsEnabled ||
                        input.StartsWith("log", StringComparison.CurrentCultureIgnoreCase) ||
                        input.StartsWith("/log", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var splitInput = input.Split(null as char[], StringSplitOptions.RemoveEmptyEntries);
                        switch (splitInput.FirstOrDefault()?.TrimStart('/').ToLowerInvariant())
                        {
                            case "unload":
                            case "exit":
                            case "quit":
                                return;
                            case "help":
                                var orderedCommands = ConsoleCommands
                                    .OrderBy(command => command.Name, StringComparer.OrdinalIgnoreCase)
                                    .ToList();
                                var nameWidth = Math.Max(orderedCommands.Max(command => command.Name.Length), 8) + 2;
                                var helpText = new StringBuilder()
                                    .AppendLine("可用命令")
                                    .AppendLine(new string('-', 24));

                                foreach (var command in orderedCommands)
                                {
                                    helpText.Append("  ")
                                        .Append(command.Name.PadRight(nameWidth))
                                        .AppendLine(command.Description);
                                }

                                CH.Info(helpText.ToString().TrimEnd());
                                break;
                            case "path":
                                var path = AppContext.BaseDirectory;
                                CH.Log("打开当前程序目录: " + path);
                                Process.Start(new ProcessStartInfo
                                {
                                    FileName = path,
                                    UseShellExecute = true
                                });
                                break;
                            case "log":
                                CH.IsEnabled = !CH.IsEnabled;
                                CH.Log(CH.IsEnabled ? "已开启日志输出" : "已关闭日志输出");
                                break;
                            case "clear":
                                CH.Clear();
                                break;
                            default:
                                CH.Warning($"未知命令: {input}");
                                break;
                        }
                    }
                    else
                    {
                        CH.Warning("Log已被关闭，请输入 log 开启");
                    }
                }
            }

            await Task.Delay(Timeout.Infinite);
        }
        catch (Exception ex)
        {
            CH.Error("程序启动失败: " + ex.Message);
            CH.Warning("按任意键退出...");
            if (CanReadInteractiveKey())
            {
                Console.ReadKey();
            }
        }
        finally
        {
            foreach (var (plugin, loader) in Enumerable.Reverse(loadedPlugins))
            {
                try
                {
                    using (BotLog.BeginScope(new ConsoleLogger($"[Plugin:{plugin.Name}]")))
                    {
                        await plugin.OnUnload();
                    }
                }
                catch (Exception ex)
                {
                    CH.Error($"插件卸载失败: {plugin.Name} - {ex.Message}");
                }

                loader.Unload();
            }

            if (adapter is not null)
            {
                try
                {
                    using (BotLog.BeginScope(adapter.Logger))
                    {
                        await adapter.StopAsync();
                    }
                }
                catch (Exception ex)
                {
                    CH.Error($"适配器停止失败: {adapter.Name} - {ex.Message}");
                }
            }

            adapterLoader?.Unload();
        }
    }

    private static bool CanReadInteractiveKey()
    {
        return Environment.UserInteractive &&
               !Console.IsInputRedirected &&
               !Console.IsOutputRedirected;
    }

    private static string ResolveConfiguredAdapterPath(string adapterRoot, string? protocol, string? commandLineAdapterPath)
    {
        if (!string.IsNullOrWhiteSpace(protocol))
        {
            var configuredPath = ResolveAdapterPath(adapterRoot, protocol);
            if (File.Exists(configuredPath))
            {
                CH.Info("按配置文件加载适配器: " + configuredPath);
                return configuredPath;
            }

            CH.Warning("配置文件指定的适配器未找到，将继续尝试命令行参数或自动扫描。");
        }

        if (string.IsNullOrWhiteSpace(commandLineAdapterPath)) return ResolveAdapterPath(adapterRoot, null);
        CH.Log("按命令行加载适配器: " + commandLineAdapterPath);
        return commandLineAdapterPath;

    }

    private static string? NormalizeOptionalPath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        return Path.IsPathRooted(path)
            ? Path.GetFullPath(path)
            : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, path));
    }

    private static PluginRoutePolicy BuildRoutePolicy(CoreConfig config)
    {
        var policy = new PluginRoutePolicy
        {
            Default = new PluginRouteRule
            {
                Mode = config.PluginRoutes.Default.Mode,
                Groups = config.PluginRoutes.Default.Groups
            }
        };

        foreach (var (pluginName, rule) in config.PluginRoutes.Plugins)
        {
            policy.Plugins[pluginName] = new PluginRouteRule
            {
                Mode = rule.Mode,
                Groups = rule.Groups
            };
        }

        return policy;
    }

    private static bool IsPluginEntryAssembly(string pluginRoot, string dllPath)
    {
        if (string.Equals(Path.GetFileName(dllPath), "QBotSharp.SDK.dll", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var parentDir = Path.GetDirectoryName(dllPath);
        if (string.IsNullOrEmpty(parentDir))
        {
            return false;
        }

        if (string.Equals(
                Path.GetFullPath(parentDir).TrimEnd(Path.DirectorySeparatorChar),
                Path.GetFullPath(pluginRoot).TrimEnd(Path.DirectorySeparatorChar),
                StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var folderName = new DirectoryInfo(parentDir).Name;
        var fileName = Path.GetFileNameWithoutExtension(dllPath);
        return string.Equals(fileName, folderName, StringComparison.OrdinalIgnoreCase);
    }

    private static string ResolveAdapterPath(string adapterRoot, string? protocol)
    {
        if (string.IsNullOrWhiteSpace(protocol))
        {
            var fallback = Directory.EnumerateFiles(adapterRoot, "*.dll", SearchOption.AllDirectories)
                .Where(dll => IsAdapterEntryAssembly(adapterRoot, dll))
                .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                .FirstOrDefault();

            return fallback ?? Path.Combine(adapterRoot, "adapter.dll");
        }

        var normalizedProtocol = protocol.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)
            ? protocol
            : protocol + ".dll";

        var directPath = Path.Combine(adapterRoot, normalizedProtocol);
        if (File.Exists(directPath))
        {
            return directPath;
        }

        var matches = Directory.EnumerateFiles(adapterRoot, normalizedProtocol, SearchOption.AllDirectories)
            .Where(dll => IsAdapterEntryAssembly(adapterRoot, dll))
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (matches.Count > 0)
        {
            return matches[0];
        }

        var fallbackAdapter = Directory.EnumerateFiles(adapterRoot, "*.dll", SearchOption.AllDirectories)
            .Where(dll => IsAdapterEntryAssembly(adapterRoot, dll))
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault();

        return fallbackAdapter ?? directPath;
    }

    private static bool IsAdapterEntryAssembly(string adapterRoot, string dllPath)
    {
        if (string.Equals(Path.GetFileName(dllPath), "QBotSharp.SDK.dll", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(Path.GetFileName(dllPath), "QBotSharp.Model.dll", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var parentDir = Path.GetDirectoryName(dllPath);
        if (string.IsNullOrEmpty(parentDir))
        {
            return false;
        }

        if (string.Equals(
                Path.GetFullPath(parentDir).TrimEnd(Path.DirectorySeparatorChar),
                Path.GetFullPath(adapterRoot).TrimEnd(Path.DirectorySeparatorChar),
                StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var folderName = new DirectoryInfo(parentDir).Name;
        var fileName = Path.GetFileNameWithoutExtension(dllPath);
        return string.Equals(fileName, folderName, StringComparison.OrdinalIgnoreCase);
    }
}
