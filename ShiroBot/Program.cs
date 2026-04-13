using System.CommandLine;
using System.Diagnostics;
using System.Text;
using ShiroBot.Core;
using ShiroBot.Hosting;
using ShiroBot.Hosting.Context;
using ShiroBot.Model.Common;
using ShiroBot.SDK.Adapter;
using ShiroBot.SDK.Abstractions;
using ShiroBot.SDK.Core;
using ShiroBot.SDK.Plugin;
using CH = ShiroBot.Core.ConsoleHelper;

namespace ShiroBot;

public static class Program
{
    private static readonly IReadOnlyList<CH.ConsoleCommandOption> ConsoleCommands =
    [
        new("help", "显示帮助信息"),
        new("plugins", "显示已加载插件"),
        new("plugin-load", "热加载指定插件"),
        new("plugin-unload", "热卸载指定插件"),
        new("path", "打开当前程序目录"),
        new("log", "切换日志输出"),
        new("clear", "清除控制台"),
        new("unload", "卸载并退出"),
        new("exit", "退出程序"),
        new("quit", "退出程序")
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

        CH.Info("ShiroBot 启动中...");
        
        var loadedPlugins = new List<LoadedPluginHandle>();
        var pluginLifecycleLock = new Lock();
        var pluginUnloadSemaphore = new SemaphoreSlim(1, 1);

        try
        {
            //load from command
            var coreConfigPath = NormalizeOptionalPath(parserResult.GetValue(configOption));
            //load from coreConfig
            var configManager = new ConfigManager(coreConfigPath);
            var coreConfig = await configManager.LoadCoreConfig();
            CH.IsEnabled = coreConfig.EnableLog;
            var groupRoutePolicy = coreConfig.PluginRoutes;
            
            var configuredProtocol = coreConfig.Protocol.Trim();
            var configuredAdapterPath = NormalizeOptionalPath(parserResult.GetValue(adapterOption));
            var configuredConsoleOption = parserResult.GetValue(noConsoleOption);

            CH.Info("配置加载完成" + (string.IsNullOrWhiteSpace(configuredProtocol)
                ? "，未配置协议，将自动选择 adapters 中的适配器"
                : "，当前协议: " + configuredProtocol));

            var adapterRoot = Path.Combine(AppContext.BaseDirectory, "adapters");
            if (!Directory.Exists(adapterRoot) ) Directory.CreateDirectory(adapterRoot);
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
            var adapterLoader = new DllLoader<IBotAdapter>();
            var adapter = adapterLoader.Load(adapterPath);
            var adapterConfigPath = ResolveAdapterConfigPath(adapterRoot, adapterPath);
            adapter.Config = ConfigContext.ForAdapter(adapterConfigPath);
            adapter.Logger = new ConsoleLogger($"[Adapter:{adapter.Name}]");
            

            var adapterMetadata = adapter.Metadata;
            CH.Log($"适配器信息: {adapterMetadata.Name} v{adapterMetadata.Version}");

            using (BotLog.BeginScope(adapter.Logger))
            {
                await adapter.StartAsync();
            }
            CH.Success("加载适配器成功: " + adapter.Name);
            
            var botContext = new BotContext(adapter, coreConfig.OwnerList, coreConfig.AdminList);
            var hostEventDispatcher = new HostEventDispatcher(pluginLifecycleLock);
            
            //get plugin folder
            var pluginRoot = NormalizeOptionalPath(parserResult.GetValue(pluginOption))
                ?? Path.Combine(AppContext.BaseDirectory, "plugins");
            if (!Directory.Exists(pluginRoot)) Directory.CreateDirectory(pluginRoot);

            BridgeAdapterEvents(
                adapter.Event,
                hostEventDispatcher,
                botContext,
                pluginRoot,
                groupRoutePolicy,
                loadedPlugins,
                pluginLifecycleLock,
                pluginUnloadSemaphore);

            CH.Info("开始加载插件...");

            await LoadPluginsAsync(
                EnumeratePluginEntryAssemblies(pluginRoot).ToList(),
                pluginRoot,
                botContext,
                hostEventDispatcher,
                groupRoutePolicy,
                loadedPlugins,
                pluginLifecycleLock);

            CH.Success("已加载插件: " + string.Join(", ", loadedPlugins.Select(p => p.Name)));

            //check if console is available or disabled by config or command line
            var hasConsole =
                Environment.UserInteractive &&
                !Console.IsInputRedirected &&
                !Console.IsOutputRedirected;
            var enableConsoleInput = hasConsole && !configuredConsoleOption && !coreConfig.DisableConsoleInput;

            switch (enableConsoleInput)
            {
                case false:
                {
                    var reasons = new List<string>();

                    if (!hasConsole)
                    {
                        reasons.Add("检测到非交互终端");
                    }

                    if (configuredConsoleOption)
                    {
                        reasons.Add("命令行参数 --no-console 已启用");
                    }

                    if (coreConfig.DisableConsoleInput)
                    {
                        reasons.Add("配置项 disable_console_input = true");
                    }

                    CH.Info($"已禁用控制台命令输入: {string.Join("，", reasons)}");
                    break;
                }
                case true:
                {
                    var exitRequested = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                    _ = Task.Run(() => RunConsoleCommandLoop(
                        exitRequested,
                        () => GetLoadedPluginNames(loadedPlugins, pluginLifecycleLock),
                        pluginName => ScheduleLoadPluginByName(
                            pluginRoot,
                            botContext,
                            hostEventDispatcher,
                            groupRoutePolicy,
                            loadedPlugins,
                            pluginLifecycleLock,
                            pluginUnloadSemaphore,
                            pluginName),
                        pluginName => ScheduleUnloadPluginByName(
                            hostEventDispatcher,
                            loadedPlugins,
                            pluginLifecycleLock,
                            pluginUnloadSemaphore,
                            pluginName)));
                    await exitRequested.Task;
                    return;
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
            foreach (var pluginHandle in Enumerable.Reverse(loadedPlugins.ToList()))
            {
                var result = await pluginHandle.UnloadAsync();
                if (result.Error is not null)
                {
                    CH.Error($"插件卸载失败: {result.Name} - {result.Error.Message}");
                }

                var assemblyUnloaded = DllLoader<IBotPlugin>.WaitForUnload(result.AssemblyLoadContextWeakReference);
                if (!assemblyUnloaded)
                {
                    CH.Warning($"插件程序集未完全卸载，可能仍有引用残留: {result.Name} ({result.AssemblyPath})");
                }
            }
        }
    }

    /*
     private method
     */
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

    private static string ResolveAdapterConfigPath(string adapterRoot, string adapterPath)
    {
        var normalizedAdapterRoot = Path.GetFullPath(adapterRoot).TrimEnd(Path.DirectorySeparatorChar);
        var parentDirectory = Path.GetDirectoryName(adapterPath) ?? adapterRoot;
        var normalizedParent = Path.GetFullPath(parentDirectory).TrimEnd(Path.DirectorySeparatorChar);
        var folderName = new DirectoryInfo(parentDirectory).Name;
        var fileName = Path.GetFileNameWithoutExtension(adapterPath);
        var isFolderBasedAdapter =
            !string.Equals(normalizedParent, normalizedAdapterRoot, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(folderName, fileName, StringComparison.OrdinalIgnoreCase);

        return !isFolderBasedAdapter ? Path.ChangeExtension(adapterPath, ".toml") : Path.Combine(normalizedParent, "config.toml");
    }

    private static void RunConsoleCommandLoop(
        TaskCompletionSource<bool> exitRequested,
        Func<IReadOnlyList<string>> getLoadedPluginNames,
        Func<string, Task> loadPluginByName,
        Func<string, Task> unloadPluginByName)
    {
        while (true)
        {
            var input = CH.ReadPrompt("> ", BuildConsoleCompletions(getLoadedPluginNames()));
            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            if (CH.IsEnabled ||
                input.StartsWith("log", StringComparison.CurrentCultureIgnoreCase))
            {
                var splitInput = input.Split(null as char[], StringSplitOptions.RemoveEmptyEntries);
                switch (splitInput.FirstOrDefault()?.ToLowerInvariant())
                {
                    case "unload":
                    case "exit":
                    case "quit":
                        exitRequested.TrySetResult(true);
                        return;
                    case "plugins":
                        var pluginNames = getLoadedPluginNames();
                        CH.Info(pluginNames.Count == 0
                            ? "当前没有已加载插件。"
                            : "已加载插件: " + string.Join(", ", pluginNames));
                        break;
                    case "plugin-load":
                        if (splitInput.Length < 2)
                        {
                            CH.Warning("用法: plugin-load <插件名|dll路径>");
                            break;
                        }

                        loadPluginByName(splitInput[1]).GetAwaiter().GetResult();
                        break;
                    case "plugin-unload":
                        if (splitInput.Length < 2)
                        {
                            CH.Warning("用法: plugin-unload <插件名>");
                            break;
                        }

                        unloadPluginByName(splitInput[1]).GetAwaiter().GetResult();
                        break;
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

    private static IReadOnlyList<CH.ConsoleCommandOption> BuildConsoleCompletions(IReadOnlyList<string> loadedPluginNames)
    {
        var completions = new List<CH.ConsoleCommandOption>(ConsoleCommands);

        foreach (var pluginName in loadedPluginNames.OrderBy(name => name, StringComparer.OrdinalIgnoreCase))
        {
            completions.Add(new CH.ConsoleCommandOption($"plugin-unload {pluginName}", $"热卸载插件 {pluginName}"));
            completions.Add(new CH.ConsoleCommandOption($"plugin-load {pluginName}", $"热加载插件 {pluginName}"));
        }

        return completions;
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

    private static IEnumerable<string> EnumeratePluginEntryAssemblies(string pluginRoot)
    {
        var sharedAssemblies = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "ShiroBot.SDK.dll",
            "ShiroBot.Model.dll"
        };
        var yieldedPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var rootLevelDlls = Directory.EnumerateFiles(pluginRoot, "*.dll", SearchOption.TopDirectoryOnly)
            .Where(dll => !sharedAssemblies.Contains(Path.GetFileName(dll)))
            .OrderBy(dll => dll, StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var dll in rootLevelDlls)
        {
            var normalizedPath = Path.GetFullPath(dll);
            if (yieldedPaths.Add(normalizedPath))
            {
                yield return normalizedPath;
            }
        }

        var pluginDirectories = Directory.EnumerateDirectories(pluginRoot)
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var normalizedPath in from directory in pluginDirectories let directoryName = new DirectoryInfo(directory).Name select Path.Combine(directory, $"{directoryName}.dll") into entryDll where File.Exists(entryDll) && !sharedAssemblies.Contains(Path.GetFileName(entryDll)) select Path.GetFullPath(entryDll) into normalizedPath where yieldedPaths.Add(normalizedPath) select normalizedPath)
        {
            yield return normalizedPath;
        }
    }

    private static List<string> GetLoadedPluginNames(List<LoadedPluginHandle> loadedPlugins, Lock pluginLifecycleLock)
    {
        lock (pluginLifecycleLock)
        {
            return loadedPlugins
                .Select(plugin => plugin.Name)
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }

    private static void BridgeAdapterEvents(
        IEventService eventService,
        HostEventDispatcher eventSink,
        BotContext botContext,
        string pluginRoot,
        PluginRouteConfig routePolicy,
        List<LoadedPluginHandle> loadedPlugins,
        Lock pluginLifecycleLock,
        SemaphoreSlim pluginLifecycleSemaphore)
    {
        eventService.GroupMessageReceived += eventSink.PublishGroupMessageAsync;
        eventService.FriendMessageReceived += message =>
            HandleFriendMessageAsync(
                message,
                eventSink,
                botContext,
                pluginRoot,
                routePolicy,
                loadedPlugins,
                pluginLifecycleLock,
                pluginLifecycleSemaphore);
        eventService.MessageRecall += eventSink.PublishMessageRecallAsync;
        eventService.FriendRequest += eventSink.PublishFriendRequestAsync;
        eventService.GroupJoinRequest += eventSink.PublishGroupJoinRequestAsync;
        eventService.GroupInvitedJoinRequest += eventSink.PublishGroupInvitedJoinRequestAsync;
        eventService.GroupInvitation += eventSink.PublishGroupInvitationAsync;
        eventService.FriendNudge += eventSink.PublishFriendNudgeAsync;
        eventService.FriendFileUpload += eventSink.PublishFriendFileUploadAsync;
        eventService.GroupAdminChange += eventSink.PublishGroupAdminChangeAsync;
        eventService.GroupEssenceMessageChange += eventSink.PublishGroupEssenceMessageChangeAsync;
        eventService.GroupMemberIncrease += eventSink.PublishGroupMemberIncreaseAsync;
        eventService.GroupMemberDecrease += eventSink.PublishGroupMemberDecreaseAsync;
        eventService.GroupNameChange += eventSink.PublishGroupNameChangeAsync;
        eventService.GroupMessageReaction += eventSink.PublishGroupMessageReactionAsync;
        eventService.GroupMute += eventSink.PublishGroupMuteAsync;
        eventService.GroupWholeMute += eventSink.PublishGroupWholeMuteAsync;
        eventService.GroupNudge += eventSink.PublishGroupNudgeAsync;
        eventService.GroupFileUpload += eventSink.PublishGroupFileUploadAsync;
    }

    private static async Task HandleFriendMessageAsync(
        FriendIncomingMessage message,
        HostEventDispatcher eventSink,
        BotContext botContext,
        string pluginRoot,
        PluginRouteConfig routePolicy,
        List<LoadedPluginHandle> loadedPlugins,
        Lock pluginLifecycleLock,
        SemaphoreSlim pluginLifecycleSemaphore)
    {
        if (await TryHandleHostPrivateCommandAsync(
                message,
                botContext,
                pluginRoot,
                routePolicy,
                loadedPlugins,
                pluginLifecycleLock,
                pluginLifecycleSemaphore,
                eventSink))
        {
            return;
        }

        await eventSink.PublishFriendMessageAsync(message);
    }

    private static async Task<bool> TryHandleHostPrivateCommandAsync(
        FriendIncomingMessage message,
        BotContext botContext,
        string pluginRoot,
        PluginRouteConfig routePolicy,
        List<LoadedPluginHandle> loadedPlugins,
        Lock pluginLifecycleLock,
        SemaphoreSlim pluginLifecycleSemaphore,
        HostEventDispatcher hostEventDispatcher)
    {
        if (!botContext.OwnerList.Contains(message.SenderId))
        {
            return false;
        }

        var input = message.GetPlainText().Trim();
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var splitInput = input.Split(null as char[], StringSplitOptions.RemoveEmptyEntries);
        if (splitInput.Length == 0)
        {
            return false;
        }

        switch (splitInput[0].ToLowerInvariant())
        {
            case "help":
            {
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

                await botContext.Message.ReplyAsync(message, helpText.ToString().TrimEnd());
                return true;
            }
            case "plugins":
            {
                var pluginNames = GetLoadedPluginNames(loadedPlugins, pluginLifecycleLock);
                await botContext.Message.ReplyAsync(
                    message,
                    
                        pluginNames.Count == 0
                            ? "当前没有已加载插件。"
                            : "已加载插件: " + string.Join(", ", pluginNames));
                return true;
            }
            case "plugin-load":
            {
                if (splitInput.Length < 2)
                {
                    await botContext.Message.ReplyAsync(message, "用法: plugin-load <插件名|dll路径>");
                    return true;
                }

                await ScheduleLoadPluginByName(
                    pluginRoot,
                    botContext,
                    hostEventDispatcher,
                    routePolicy,
                    loadedPlugins,
                    pluginLifecycleLock,
                    pluginLifecycleSemaphore,
                    splitInput[1]);
                await botContext.Message.ReplyAsync(message, $"已加入热加载队列: {splitInput[1]}");
                return true;
            }
            case "plugin-unload":
            {
                if (splitInput.Length < 2)
                {
                    await botContext.Message.ReplyAsync(message, "用法: plugin-unload <插件名>");
                    return true;
                }

                await ScheduleUnloadPluginByName(
                    hostEventDispatcher,
                    loadedPlugins,
                    pluginLifecycleLock,
                    pluginLifecycleSemaphore,
                    splitInput[1]);
                await botContext.Message.ReplyAsync(message, $"已加入热卸载队列: {splitInput[1]}");
                return true;
            }
            default:
                return false;
        }
    }

    private static Task ScheduleUnloadPluginByName(
        HostEventDispatcher hostEventDispatcher,
        List<LoadedPluginHandle> loadedPlugins,
        Lock pluginLifecycleLock,
        SemaphoreSlim pluginUnloadSemaphore,
        string pluginName)
    {
        LoadedPluginHandle? pluginHandle;
        lock (pluginLifecycleLock)
        {
            pluginHandle = loadedPlugins.FirstOrDefault(plugin =>
                string.Equals(plugin.Name, pluginName, StringComparison.OrdinalIgnoreCase));

            if (pluginHandle is not null)
            {
                loadedPlugins.Remove(pluginHandle);
                hostEventDispatcher.UnregisterPlugin(pluginHandle);
            }
        }

        if (pluginHandle is null)
        {
            CH.Warning($"未找到已加载插件: {pluginName}");
            return Task.CompletedTask;
        }

        CH.Info($"已加入热卸载队列: {pluginHandle.Name}");
        _ = Task.Run(() => ProcessPluginUnloadAsync(pluginHandle, pluginUnloadSemaphore));

        return Task.CompletedTask;
    }

    private static async Task ProcessPluginUnloadAsync(
        LoadedPluginHandle pluginHandle,
        SemaphoreSlim pluginUnloadSemaphore)
    {
        await pluginUnloadSemaphore.WaitAsync();
        try
        {
            CH.Info($"开始热卸载插件: {pluginHandle.Name}");
            var result = await pluginHandle.UnloadAsync();

            if (result.Error is not null)
            {
                CH.Error($"插件卸载失败: {result.Name} - {result.Error.Message}");
                return;
            }

            CH.Info($"插件逻辑已卸载，正在后台验证程序集释放: {result.Name}");
            await Task.Delay(300);

            var assemblyUnloaded = DllLoader<IBotPlugin>.WaitForUnload(result.AssemblyLoadContextWeakReference);
            if (!assemblyUnloaded)
            {
                var aliveObjects = new List<string>();
                if (result.PluginWeakReference?.IsAlive == true)
                {
                    aliveObjects.Add("plugin");
                }

                if (result.ContextWeakReference?.IsAlive == true)
                {
                    aliveObjects.Add("plugin-context");
                }

                if (aliveObjects.Count > 0)
                {
                    CH.Warning($"热卸载诊断: {result.Name} 存活对象: {string.Join(", ", aliveObjects)}");
                }

                CH.Warning($"插件逻辑已卸载，但程序集仍有残留引用: {result.Name} ({result.AssemblyPath})");
                return;
            }

            CH.Success($"插件热卸载成功: {result.Name}");
        }
        finally
        {
            pluginUnloadSemaphore.Release();
        }
    }

    private static Task ScheduleLoadPluginByName(
        string pluginRoot,
        BotContext botContext,
        HostEventDispatcher hostEventDispatcher,
        PluginRouteConfig routePolicy,
        List<LoadedPluginHandle> loadedPlugins,
        Lock pluginLifecycleLock,
        SemaphoreSlim pluginLifecycleSemaphore,
        string pluginNameOrPath)
    {
        var candidateAssemblies = ResolvePluginLoadCandidates(pluginRoot, pluginNameOrPath);
        if (candidateAssemblies.Count == 0)
        {
            CH.Warning($"未找到可加载插件: {pluginNameOrPath}");
            return Task.CompletedTask;
        }

        CH.Info($"已加入热加载队列: {pluginNameOrPath}");
        _ = Task.Run(() => ProcessPluginLoadAsync(
            candidateAssemblies,
            pluginRoot,
            botContext,
            hostEventDispatcher,
            routePolicy,
            loadedPlugins,
            pluginLifecycleLock,
            pluginLifecycleSemaphore));

        return Task.CompletedTask;
    }

    private static async Task ProcessPluginLoadAsync(
        IReadOnlyList<string> candidateAssemblies,
        string pluginRoot,
        BotContext botContext,
        HostEventDispatcher hostEventDispatcher,
        PluginRouteConfig routePolicy,
        List<LoadedPluginHandle> loadedPlugins,
        Lock pluginLifecycleLock,
        SemaphoreSlim pluginLifecycleSemaphore)
    {
        await pluginLifecycleSemaphore.WaitAsync();
        try
        {
            await LoadPluginsAsync(
                candidateAssemblies,
                pluginRoot,
                botContext,
                hostEventDispatcher,
                routePolicy,
                loadedPlugins,
                pluginLifecycleLock);
        }
        finally
        {
            pluginLifecycleSemaphore.Release();
        }
    }

    private static async Task LoadPluginsAsync(
        IReadOnlyList<string> pluginDlls,
        string pluginRoot,
        BotContext botContext,
        HostEventDispatcher hostEventDispatcher,
        PluginRouteConfig routePolicy,
        List<LoadedPluginHandle> loadedPlugins,
        Lock pluginLifecycleLock)
    {
        foreach (var dll in pluginDlls)
        {
            await LoadPluginAsync(dll, pluginRoot, botContext, hostEventDispatcher, routePolicy, loadedPlugins, pluginLifecycleLock);
        }
    }

    private static async Task LoadPluginAsync(
        string dll,
        string pluginRoot,
        BotContext botContext,
        HostEventDispatcher hostEventDispatcher,
        PluginRouteConfig routePolicy,
        List<LoadedPluginHandle> loadedPlugins,
        Lock pluginLifecycleLock)
    {
        DllLoader<IBotPlugin>? loader = null;
        try
        {
            var actualDllPath = Path.GetFullPath(dll);
            
            var isRootLevelPluginAssembly = string.Equals(
                Path.GetFullPath(Path.GetDirectoryName(dll) ?? pluginRoot).TrimEnd(Path.DirectorySeparatorChar),
                Path.GetFullPath(pluginRoot).TrimEnd(Path.DirectorySeparatorChar),
                StringComparison.OrdinalIgnoreCase);
            
            if (isRootLevelPluginAssembly)
            {
                var pluginName = ProbePluginName(dll);
                actualDllPath = RelocateRootPluginAssembly(pluginRoot, dll, pluginName);
            }

            loader = new DllLoader<IBotPlugin>();
            var plugin = loader.Load(actualDllPath);

            lock (pluginLifecycleLock)
            {
                if (loadedPlugins.Any(item => string.Equals(item.Name, plugin.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    CH.Warning($"插件名重复，已跳过：{plugin.Name} ({actualDllPath})");
                    loader.Unload();
                    return;
                }
            }

            var pluginDirectory = ResolvePluginDirectory(pluginRoot, actualDllPath, plugin.Name);
            var pluginContext = new PluginContext(
                botContext,
                plugin.Name,
                pluginDirectory,
                groupId => routePolicy.AllowsGroup(plugin.Name, groupId));

            var metadata = plugin.Metadata;
            CH.Info($"开始加载插件: {metadata.Name} v{metadata.Version} ");

            using (BotLog.BeginScope(pluginContext.Logger))
            {
                await plugin.OnLoad(pluginContext);
            }

            var pluginHandle = new LoadedPluginHandle(
                plugin,
                pluginContext,
                loader,
                actualDllPath,
                groupId => routePolicy.AllowsGroup(plugin.Name, groupId));

            lock (pluginLifecycleLock)
            {
                loadedPlugins.Add(pluginHandle);
                hostEventDispatcher.RegisterPlugin(pluginHandle);
            }

            CH.Success($"插件加载成功: {plugin.Name} ({actualDllPath})");
            loader = null;
        }
        catch (Exception ex)
        {
            CH.Error($"插件加载失败: {dll} - {ex.Message}");
            loader?.Unload();
        }
    }

    private static IReadOnlyList<string> ResolvePluginLoadCandidates(string pluginRoot, string pluginNameOrPath)
    {
        var normalizedInput = pluginNameOrPath.Trim();

        if (Path.HasExtension(normalizedInput) || normalizedInput.Contains(Path.DirectorySeparatorChar) || normalizedInput.Contains(Path.AltDirectorySeparatorChar))
        {
            var fullPath = Path.IsPathRooted(normalizedInput)
                ? Path.GetFullPath(normalizedInput)
                : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, normalizedInput));

            return File.Exists(fullPath) ? [fullPath] : [];
        }

        var directDll = Path.Combine(pluginRoot, $"{normalizedInput}.dll");
        if (File.Exists(directDll))
        {
            return [Path.GetFullPath(directDll)];
        }

        var pluginDirectoryDll = Path.Combine(pluginRoot, normalizedInput, $"{normalizedInput}.dll");
        if (File.Exists(pluginDirectoryDll))
        {
            return [Path.GetFullPath(pluginDirectoryDll)];
        }

        return EnumeratePluginEntryAssemblies(pluginRoot)
            .Where(path =>
            {
                var fileName = Path.GetFileNameWithoutExtension(path);
                var directoryName = new DirectoryInfo(Path.GetDirectoryName(path) ?? pluginRoot).Name;
                return string.Equals(fileName, normalizedInput, StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(directoryName, normalizedInput, StringComparison.OrdinalIgnoreCase);
            })
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static string ResolvePluginDirectory(string pluginRoot, string dllPath, string pluginName)
    {
        var parentDir = Path.GetDirectoryName(dllPath) ?? pluginRoot;
        var normalizedRoot = Path.GetFullPath(pluginRoot).TrimEnd(Path.DirectorySeparatorChar);
        var normalizedParent = Path.GetFullPath(parentDir).TrimEnd(Path.DirectorySeparatorChar);

        if (!string.Equals(normalizedParent, normalizedRoot, StringComparison.OrdinalIgnoreCase))
        {
            return parentDir;
        }

        var pluginDirectory = Path.Combine(pluginRoot, pluginName);
        Directory.CreateDirectory(pluginDirectory);
        return pluginDirectory;
    }

    private static string ProbePluginName(string dllPath)
    {
        var probeDirectory = Path.Combine(Path.GetTempPath(), "ShiroBot", "plugin-probe", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(probeDirectory);

        var probeDllPath = Path.Combine(probeDirectory, Path.GetFileName(dllPath));
        File.Copy(dllPath, probeDllPath, overwrite: true);

        DllLoader<IBotPlugin>? loader = null;
        try
        {
            loader = new DllLoader<IBotPlugin>();
            var plugin = loader.Load(probeDllPath);
            return plugin.Name;
        }
        finally
        {
            loader?.Unload();
            TryDeleteDirectory(probeDirectory);
        }
    }

    private static string RelocateRootPluginAssembly(string pluginRoot, string dllPath, string pluginName)
    {
        var pluginDirectory = Path.Combine(pluginRoot, pluginName);
        Directory.CreateDirectory(pluginDirectory);

        var targetDllPath = Path.Combine(pluginDirectory, $"{pluginName}.dll");
        CopyPluginArtifact(dllPath, targetDllPath, deleteSource: true);

        var sourceBasePath = Path.Combine(
            Path.GetDirectoryName(dllPath) ?? pluginRoot,
            Path.GetFileNameWithoutExtension(dllPath));
        var targetBasePath = Path.Combine(pluginDirectory, pluginName);
        
        CopyPluginArtifact(sourceBasePath + ".runtimeconfig.json", targetBasePath + ".runtimeconfig.json", deleteSource: true);
        CopyPluginArtifact(sourceBasePath + ".toml", Path.Combine(pluginDirectory, "config.toml"), deleteSource: true);

        return targetDllPath;
    }

    private static void CopyPluginArtifact(string sourcePath, string targetPath, bool deleteSource)
    {
        if (!File.Exists(sourcePath))
        {
            return;
        }

        var normalizedSource = Path.GetFullPath(sourcePath);
        var normalizedTarget = Path.GetFullPath(targetPath);
        if (string.Equals(normalizedSource, normalizedTarget, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(normalizedTarget)!);
        ExecuteFileOperationWithRetry(() => File.Copy(normalizedSource, normalizedTarget, overwrite: true));

        if (!deleteSource) return;
        if (!File.Exists(normalizedSource))
        {
            return;
        }

        const int maxAttempts = 5;
        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                File.Delete(normalizedSource);
                return;
            }
            catch (IOException) when (attempt < maxAttempts)
            {
                Thread.Sleep(100 * attempt);
            }
            catch (UnauthorizedAccessException) when (attempt < maxAttempts)
            {
                Thread.Sleep(100 * attempt);
            }
            catch (IOException)
            {
                return;
            }
            catch (UnauthorizedAccessException)
            {
                return;
            }
        }
    }

    private static void ExecuteFileOperationWithRetry(Action action)
    {
        const int maxAttempts = 5;
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                action();
                return;
            }
            catch (IOException) when (attempt < maxAttempts)
            {
                Thread.Sleep(100 * attempt);
            }
            catch (UnauthorizedAccessException) when (attempt < maxAttempts)
            {
                Thread.Sleep(100 * attempt);
            }
        }
    }

    private static void TryDeleteDirectory(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            return;
        }

        const int maxAttempts = 5;
        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                Directory.Delete(directoryPath, recursive: true);
                return;
            }
            catch (IOException) when (attempt < maxAttempts)
            {
                Thread.Sleep(100 * attempt);
            }
            catch (UnauthorizedAccessException) when (attempt < maxAttempts)
            {
                Thread.Sleep(100 * attempt);
            }
            catch (IOException)
            {
                return;
            }
            catch (UnauthorizedAccessException)
            {
                return;
            }
        }
        
    }
    

    private static string ResolveAdapterPath(string adapterRoot, string? protocol)
    {
        if (string.IsNullOrWhiteSpace(protocol))
        {
            var fallback = EnumerateAdapterEntryAssemblies(adapterRoot)
                .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                .FirstOrDefault();

            return fallback ?? Path.Combine(adapterRoot, "adapter.dll");
        }

        foreach (var candidate in GetAdapterPathCandidates(adapterRoot, protocol))
        {
            if (File.Exists(candidate) && IsAdapterEntryAssembly(adapterRoot, candidate))
            {
                return candidate;
            }
        }

        var fallbackAdapter = EnumerateAdapterEntryAssemblies(adapterRoot)
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault();

        return fallbackAdapter ?? GetAdapterPathCandidates(adapterRoot, protocol).First();
    }

    private static IEnumerable<string> EnumerateAdapterEntryAssemblies(string adapterRoot)
    {
        var rootLevelDlls = Directory.EnumerateFiles(adapterRoot, "*.dll", SearchOption.TopDirectoryOnly)
            .Where(dll => IsAdapterEntryAssembly(adapterRoot, dll))
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase);

        foreach (var dll in rootLevelDlls)
        {
            yield return dll;
        }

        var adapterDirectories = Directory.EnumerateDirectories(adapterRoot)
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase);

        foreach (var directory in adapterDirectories)
        {
            var directoryName = new DirectoryInfo(directory).Name;
            var entryDll = Path.Combine(directory, $"{directoryName}.dll");

            if (File.Exists(entryDll) && IsAdapterEntryAssembly(adapterRoot, entryDll))
            {
                yield return entryDll;
            }
        }
    }

    private static IReadOnlyList<string> GetAdapterPathCandidates(string adapterRoot, string protocol)
    {
        var normalizedProtocol = protocol.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)
            ? protocol
            : protocol + ".dll";
        var protocolName = Path.GetFileNameWithoutExtension(normalizedProtocol);

        return
        [
            Path.Combine(adapterRoot, normalizedProtocol),
            Path.Combine(adapterRoot, protocolName, normalizedProtocol),
            Path.Combine(adapterRoot, protocolName, $"{protocolName}.dll")
        ];
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
