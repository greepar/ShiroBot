using System.CommandLine;
using System.Diagnostics;
using System.Text;
using ShiroBot.Core;
using ShiroBot.Hosting;
using ShiroBot.Hosting.Context;
using ShiroBot.SDK;
using ShiroBot.SDK.Abstractions;
using ShiroBot.SDK.Core;
using CH = ShiroBot.Core.ConsoleHelper;

namespace ShiroBot;

public static class Program
{
    private static readonly IReadOnlyList<ConsoleHelper.ConsoleCommandOption> ConsoleCommands =
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

        var rootCommand = new RootCommand("ShiroBot 主程序")
        {
            adapterOption,
            pluginOption,
            configOption,
            noConsoleOption
        };

        var parserResult = rootCommand.Parse(args);

        ConsoleHelper.Info("ShiroBot 启动中...");

        DllLoader<IBotAdapter>? adapterLoader = null;
        var loadedPlugins = new List<(IBotPlugin Plugin, DllLoader<IBotPlugin> Loader)>();

        try
        {
            //load from command
            var configuredConfigPath = NormalizeOptionalPath(parserResult.GetValue(configOption));
            //load from config
            var configManager = new ConfigManager(configuredConfigPath);
            var coreConfig = await configManager.LoadCoreConfig();
            ConsoleHelper.IsEnabled = coreConfig.EnableLog;
            var routePolicy = coreConfig.PluginRoutes;
            
            var configuredProtocol = coreConfig.Protocol.Trim();
            var configuredAdapterPath = NormalizeOptionalPath(parserResult.GetValue(adapterOption));
            var disableConsoleFromArg = parserResult.GetValue(noConsoleOption);

            ConsoleHelper.Info("配置加载完成" + (string.IsNullOrWhiteSpace(configuredProtocol)
                ? "，未配置协议，将自动选择 adapters 中的适配器"
                : "，当前协议: " + configuredProtocol));

            var adapterRoot = Path.Combine(AppContext.BaseDirectory, "adapters");
            Directory.CreateDirectory(adapterRoot);

            var adapterPath = ResolveConfiguredAdapterPath(adapterRoot, configuredProtocol, configuredAdapterPath);
            if (!File.Exists(adapterPath))
            {
                ConsoleHelper.Warning("未找到适配器文件: " + adapterPath);
                ConsoleHelper.Warning("请确认 adapters 目录下存在对应的适配器文件，或在 config.toml 中配置 protocol...");
                if (CanReadInteractiveKey())
                {
                    Console.ReadKey();
                }
                return;
            }

            ConsoleHelper.Log("开始加载适配器: " + adapterPath);
            adapterLoader = new DllLoader<IBotAdapter>();
            var adapter = adapterLoader.Load(adapterPath);

            var adapterDirectory = Path.GetDirectoryName(adapterPath) ?? AppContext.BaseDirectory;
            adapter.Config = ConfigContext.ForAdapter(adapterDirectory);
            adapter.Logger = new ConsoleLogger($"[Adapter:{adapter.Name}]");

            var adapterMetadata = adapter.Metadata;
            ConsoleHelper.Log($"适配器元数据: {adapterMetadata.Name} v{adapterMetadata.Version}");

            using (BotLog.BeginScope(adapter.Logger))
            {
                await adapter.StartAsync();
            }
            HostEventLogger.Attach(adapter.Event);
            ConsoleHelper.Success("加载成功,适配器: " + adapter.Name);

            var pluginRoot = NormalizeOptionalPath(parserResult.GetValue(pluginOption))
                ?? Path.Combine(AppContext.BaseDirectory, "plugins");
            Directory.CreateDirectory(pluginRoot);

            ConsoleHelper.Info("开始加载插件...");

            var botContext = new BotContext(adapter);
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
                    var pluginContext = new PluginContext(
                        botContext,
                        adapter,
                        plugin.Name,
                        Path.GetDirectoryName(dll),
                        groupId => routePolicy.AllowsGroup(plugin.Name, groupId));

                    if (!loadedNames.Add(plugin.Name))
                    {
                        ConsoleHelper.Warning($"插件名重复，已跳过：{plugin.Name} ({dll})");
                        loader.Unload();
                        continue;
                    }

                    var metadata = plugin.Metadata;
                    ConsoleHelper.Info($"插件元数据: {metadata.Name} v{metadata.Version} ");

                    using (BotLog.BeginScope(pluginContext.Logger))
                    {
                        await plugin.OnLoad(pluginContext);
                    }
                    loadedPlugins.Add((plugin, loader));
                    ConsoleHelper.Success($"插件加载成功: {plugin.Name} ({dll})");
                }
                catch (Exception ex)
                {
                    ConsoleHelper.Error($"插件加载失败: {dll} - {ex.Message}");
                }
            }

            ConsoleHelper.Success("已加载插件: " + string.Join(", ", loadedPlugins.Select(p => p.Plugin.Name)));

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

                ConsoleHelper.Info($"已禁用控制台命令输入: {string.Join("，", reasons)}");
            }

            if (enableConsoleInput)
            {
                var exitRequested = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                _ = Task.Run(() => RunConsoleCommandLoop(exitRequested));
                await exitRequested.Task;
                return;
            }

            await Task.Delay(Timeout.Infinite);
        }
        catch (Exception ex)
        {
            ConsoleHelper.Error("程序启动失败: " + ex.Message);
            ConsoleHelper.Warning("按任意键退出...");
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
                    ConsoleHelper.Error($"插件卸载失败: {plugin.Name} - {ex.Message}");
                }

                loader.Unload();
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
                ConsoleHelper.Info("按配置文件加载适配器: " + configuredPath);
                return configuredPath;
            }

            ConsoleHelper.Warning("配置文件指定的适配器未找到，将继续尝试命令行参数或自动扫描。");
        }

        if (string.IsNullOrWhiteSpace(commandLineAdapterPath)) return ResolveAdapterPath(adapterRoot, null);
        ConsoleHelper.Log("按命令行加载适配器: " + commandLineAdapterPath);
        return commandLineAdapterPath;

    }

    private static void RunConsoleCommandLoop(TaskCompletionSource<bool> exitRequested)
    {
        while (true)
        {
            var input = ConsoleHelper.ReadPrompt("> ", ConsoleCommands);
            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            if (ConsoleHelper.IsEnabled ||
                input.StartsWith("log", StringComparison.CurrentCultureIgnoreCase) ||
                input.StartsWith("/log", StringComparison.CurrentCultureIgnoreCase))
            {
                var splitInput = input.Split(null as char[], StringSplitOptions.RemoveEmptyEntries);
                switch (splitInput.FirstOrDefault()?.TrimStart('/').ToLowerInvariant())
                {
                    case "unload":
                    case "exit":
                    case "quit":
                        exitRequested.TrySetResult(true);
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

                        ConsoleHelper.Info(helpText.ToString().TrimEnd());
                        break;
                    case "path":
                        var path = AppContext.BaseDirectory;
                        ConsoleHelper.Log("打开当前程序目录: " + path);
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = path,
                            UseShellExecute = true
                        });
                        break;
                    case "log":
                        ConsoleHelper.IsEnabled = !ConsoleHelper.IsEnabled;
                        ConsoleHelper.Log(ConsoleHelper.IsEnabled ? "已开启日志输出" : "已关闭日志输出");
                        break;
                    case "clear":
                        ConsoleHelper.Clear();
                        break;
                    default:
                        ConsoleHelper.Warning($"未知命令: {input}");
                        break;
                }
            }
            else
            {
                ConsoleHelper.Warning("Log已被关闭，请输入 log 开启");
            }
        }
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

    private static bool IsPluginEntryAssembly(string pluginRoot, string dllPath)
    {
        if (string.Equals(Path.GetFileName(dllPath), "ShiroBot.SDK.dll", StringComparison.OrdinalIgnoreCase))
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
        if (string.Equals(Path.GetFileName(dllPath), "ShiroBot.SDK.dll", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(Path.GetFileName(dllPath), "ShiroBot.Model.dll", StringComparison.OrdinalIgnoreCase))
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
