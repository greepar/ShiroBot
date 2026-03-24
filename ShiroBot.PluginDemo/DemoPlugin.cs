using System.Text;
using EmbedIO;
using ShiroBot.Model.Common;
using ShiroBot.PluginDemo.Services.Puppeteer;
using ShiroBot.PluginDemo.Services.Minecraft;
using ShiroBot.SDK;
using ShiroBot.SDK.Abstractions;
using ShiroBot.SDK.Plugin;
using Swan.Logging;

namespace ShiroBot.PluginDemo;

public class DemoPlugin : PluginBase
{
    private readonly MinecraftStatusService _minecraftStatusService = new();
    private WebServer? _server;
    private IDisposable? _configWatcher;
    private DemoPluginConfig _config = new();

    public override string Name => "DemoPlugin";
    public override BotComponentMetadata Metadata { get; } = new()
    {
        Name = "DemoPlugin",
        Version = "1.0.0",
        Description = "示例插件"
    };

    protected override async Task OnLoadAsync(IBotContext context)
    {
        _config = context.Config.Load<DemoPluginConfig>();
        context.Config.Save(_config);
        BotLog.Info($"插件加载完成，热重载={_config.EnableHotReload}，允许群数量={_config.AllowGroups.Length}");
        _server = new WebServer(o => o
                .WithUrlPrefix("http://localhost:8080")
                .WithMode(HttpListenerMode.EmbedIO))
            .WithAction("/", HttpVerbs.Get, ctx =>
                ctx.SendStringAsync("Hello World!", "text/plain", Encoding.UTF8));
        Logger.NoLogging();
        _ = _server.RunAsync();


        await Task.Delay(1);
        // await BrowserManager.GetBrowserAsync();
        if (_config.SendStartupHello)
        {
            // await context.Message.SendPrivateTextAsync(1034028486, $"hello from plugin;{Name}");
        }
        
        FriendCommands.Map("#help", HandleFriendHelpAsync);
        FriendCommands.Map("#status", async message =>
        {
            await Context.Message.ReplyTextAsync(message, $"当前时间: {DateTime.Now}");
        });

        GroupCommands.Map("#status", async message =>
        {
            var fullText = message.GetPlainText().Trim();
            var args = fullText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (args.Length < 2)
            {
                await Context.Message.ReplyTextAsync(message, "用法: #status <服务器[:端口]>");
                return;
            }

            var targetServer = args[1];
            BotLog.Info($"收到 Minecraft 状态查询请求: group={message.Group.GroupId}, target={targetServer}");

            try
            {
                var status = await _minecraftStatusService.QueryAsync(targetServer);
                BotLog.Info(
                    $"Minecraft 查询成功: target={status.Host}:{status.Port}, online={status.OnlinePlayers}/{status.MaxPlayers}, latency={status.LatencyMs}ms");
                var imageBytes = MinecraftStatusCardRenderer.Render(status);
                BotLog.Info($"Minecraft 状态卡渲染完成: {imageBytes.Length} bytes");
                var base64 = Convert.ToBase64String(imageBytes);

                await Context.Message.SendGroupMessageAsync(
                    message.Group.GroupId,
                    [new ImageOutgoingSegment($"base64://{base64}")]);
                BotLog.Success($"Minecraft 状态卡已发送: group={message.Group.GroupId}, target={targetServer}");
            }
            catch (Exception ex)
            {
                BotLog.Warning($"Minecraft 状态查询失败: {ex.Message}");
                await Context.Message.ReplyTextAsync(message, $"查询失败: {ex.Message}");
            }
        });
        
        if (_config.EnableHotReload)
        {
            _configWatcher = context.Config.Watch<DemoPluginConfig>(updated =>
            {
                _config = updated;
                BotLog.Info($"插件 {Name} 配置已热重载: {context.Config.ConfigPath}");
            });
        }
    }

    protected override Task OnUnloadAsync()
    {
        BotLog.Info("插件开始卸载");
        _configWatcher?.Dispose();
        _configWatcher = null;
        _server?.Dispose();
        _server = null;
        return Task.CompletedTask;
    }
    
    protected override Task<bool> BeforeDispatchGroupCommandAsync(GroupIncomingMessage message) =>
        Task.FromResult(_config.AllowGroups.Contains(message.Group.GroupId));

    protected override Task OnGroupFileUploadAsync(GroupFileUploadEvent e)
    {
        BotLog.Info($"收到群文件上传事件: {e.GroupId} / {e.FileName}");
        return Task.CompletedTask;
    }

    protected override Task OnFriendFileUploadAsync(FriendFileUploadEvent e)
    {
        return base.OnFriendFileUploadAsync(e);
    }


    private Task HandleFriendStatusAsync(FriendIncomingMessage message) =>
        Context.Message.ReplyTextAsync(message, $"当前时间: {DateTime.Now}");

    private async Task HandleFriendHelpAsync(FriendIncomingMessage message) {
       await Context.Message.ReplyTextAsync(message, "可用命令: #status, #help");
    }
     
}
