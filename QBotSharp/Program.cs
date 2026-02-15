using System.CommandLine;
using System.Diagnostics;
using Milky.Net.Model;
using QBotSharp.Hosting.BotContext;
using QBotSharp.SDK;
using QBotSharp.Utils;
using Spectre.Console;
using CH = QBotSharp.Utils.ConsoleHelper;

namespace QBotSharp;

public static class Program
{
    public static async Task Main(string[] args)
    {
        // 定义命令行选项 - 使用构造函数的 aliases 参数
        var adapterOption = new Option<string?>(
            "--adapter")
        {
            Description = "指定适配器 DLL 文件路径（可以是相对路径或绝对路径）"
        };

        var pluginOption = new Option<string?>(
            "--plugin-dir")
        {
            Description = "指定插件文件夹路径（可以是相对路径或绝对路径）"
        };

        var configOption = new Option<string?>(
            "--config",
            "-c")
        {
            Description = "指定配置文件路径"
        };

        var rootCommand = new RootCommand("QBotSharp 主程序")
        {
            adapterOption,
            pluginOption,
            configOption
        };

        var parserResult = rootCommand.Parse(args);


        CH.Info("QBotSharp 启动中...");
        try
        {
            //临时
            var config = new ConfigManager();
            var coreConfig = await config.LoadCoreConfig();
            var protocol = coreConfig.Protocol.ToLower().EndsWith(".dll")
                ? coreConfig.Protocol
                : coreConfig.Protocol + ".dll";
            CH.Info("配置加载完成，当前协议: " + coreConfig.Protocol);

            string? adapterPath = null;
            if (parserResult.GetValue(adapterOption)?.TrimEnd() is not null)
            {
                adapterPath = parserResult.GetValue(adapterOption);
                CH.Info("命令行指定适配器路径: " + adapterPath);
            }

            var adapterRoot = Path.Combine(AppContext.BaseDirectory, "adapters");
            if (!Directory.Exists(adapterRoot) && adapterPath is null)
            {
                Directory.CreateDirectory(adapterRoot);
                CH.Warning("未找到 adapters 目录，已自动创建");
                CH.Warning("请放入adapter后在config.toml配置adapter文件...");
                Console.ReadKey();
                return;
            }

            adapterPath ??= Path.Combine(adapterRoot, protocol);
            if (!File.Exists(adapterPath))
            {
                CH.Warning("未找到适配器文件: " + adapterPath);
                CH.Warning("请确认 adapters 目录下存在对应的适配器文件并在config.toml中配置...");
                Console.ReadKey();
                return;
            }

            CH.Info("开始加载适配器: " + adapterPath);
            var adapterLoader = new DllLoader<IBotAdapter>();
            var adapter = adapterLoader.Load(adapterPath);
            var context = new BotContext(adapter);
            await adapter.StartAsync();
            CH.Success("加载成功,适配器: " + adapter.Name);
            var segments = new OutgoingSegment[]
            {
                new TextOutgoingSegment("hello")
            };
            var request = new SendPrivateMessageRequest(1034028486, segments);
            //测试发送私聊消息
            await adapter.Message.SendPrivateMessageAsync(request);

            //加载Plugin
            var pluginRoot = Path.Combine(AppContext.BaseDirectory, "plugins");
            if (!Directory.Exists(pluginRoot))
            {
                CH.Warning("未找到 adapters 目录，已自动创建");
                Directory.CreateDirectory(pluginRoot);
            }

            CH.Info("开始加载插件...");
            var plugins = new List<IBotPlugin>();
            var pluginLoaders = new List<DllLoader<IBotPlugin>>();
            var loadedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var dll in Directory.EnumerateFiles(
                         pluginRoot, "*.dll", SearchOption.TopDirectoryOnly))
                try
                {
                    var loader = new DllLoader<IBotPlugin>();
                    var plugin = loader.Load(dll);

                    if (!loadedNames.Add(plugin.Name))
                    {
                        CH.Warning($"插件名重复，已跳过：{plugin.Name} ({dll})");
                        loader.Unload(); // 非常重要，防止程序集常驻
                        continue;
                    }

                    plugins.Add(plugin);
                    pluginLoaders.Add(loader); // 必须保存，用于卸载

                    await plugin.OnLoad(context); // 这里传入实际的BotContext
                    Console.WriteLine($"Loaded plugin: {dll}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load plugin {dll}: {ex.Message}");
                }

            var pluginNames = plugins.Select(p => p.Name).ToList();
            CH.Info("已加载插件: " + string.Join(", ", pluginNames));

            //开始构造BotHost
            var botHost = await BotHostBuilder
                .CreateDefault()
                .BuildAsync();
            await botHost.RunAsync();


            var hasConsole =
                Environment.UserInteractive &&
                !Console.IsInputRedirected &&
                !Console.IsOutputRedirected;
            //开始交互控制台
            if (hasConsole)
                while (true)
                {
                    var input = AnsiConsole.Prompt(
                        new TextPrompt<string>("> ")
                            .AllowEmpty());

                    if (CH.IsEnabled || input.StartsWith("log", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var splitInput = input.Split(null as char[], StringSplitOptions.RemoveEmptyEntries);
                        switch (splitInput.FirstOrDefault()?.ToLower())
                        {
                            case "unload":
                                adapterLoader.Unload();
                                break;
                            case "help":

                                CH.Log("可用命令:");
                                CH.Log("  help       显示此帮助信息");
                                CH.Log("  path       打开当前程序目录");
                                CH.Log("  log        切换日志输出开关");
                                CH.Log("  clear      清除控制台");
                                CH.Log("  exit/quit  退出程序");
                                break;

                            case "path":
                                var path = AppContext.BaseDirectory;
                                AnsiConsole.MarkupLine(
                                    $"[grey][[{DateTime.Now:HH:mm:ss}]][/] 打开当前程序目录: [blue]{path}[/]");
                                Process.Start(new ProcessStartInfo
                                {
                                    FileName = path,
                                    UseShellExecute = true
                                });
                                break;
                            case "log":
                                CH.IsEnabled = !CH.IsEnabled;
                                var statusText = CH.IsEnabled ? "[green]已开启日志输出[/]" : "[red]已关闭日志输出[/]";
                                AnsiConsole.MarkupLine($"[grey][[{DateTime.Now:HH:mm:ss}]][/] {statusText}");
                                break;
                            case "exit":
                            case "quit":
                                return;
                            case "clear":
                                Console.Clear();
                                break;
                            default:
                                CH.Warning($"未知命令: {input}");
                                break;
                        }
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"[grey][[{DateTime.Now:HH:mm:ss}]][/] [yellow]Log已被关闭，请输入log on开启[/]");
                    }
                }

            await Task.Delay(Timeout.Infinite);
        }
        catch (Exception ex)
        {
            CH.Error("程序启动失败: " + ex.Message);
            //按任意键退出
            CH.Warning("按任意键退出...");
            Console.ReadKey();
        }
    }
}