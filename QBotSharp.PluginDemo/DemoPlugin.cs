using System.Text;
using EmbedIO;
using QBotSharp.Model.Common;
using QBotSharp.PluginDemo.Services.Puppeteer;
using QBotSharp.SDK;
using QBotSharp.SDK.Plugin;
using Swan.Logging;

namespace QBotSharp.PluginDemo;

public class DemoPlugin : PluginBase
{
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
        Logger.NoLogging();
        _server = new WebServer(o => o
                .WithUrlPrefix("http://localhost:8080")
                .WithMode(HttpListenerMode.EmbedIO))
            .WithAction("/", HttpVerbs.Get, ctx =>
                ctx.SendStringAsync("Hello World!", "text/plain", Encoding.UTF8));
        _ = _server.RunAsync();


        await BrowserManager.GetBrowserAsync();
        if (_config.SendStartupHello)
        {
            // await context.Message.SendPrivateTextAsync(1034028486, $"hello from plugin;{Name}");
        }
        
        FriendCommands.Map("#help", HandleFriendHelpAsync);
        FriendCommands.Map("#status", async message =>
        {
            BotLog.Info($"测试");
            await Context.Message.ReplyTextAsync(message, $"当前时间: {DateTime.Now}");
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
        
        BotLog.Error("HI");
       await Context.Message.ReplyTextAsync(message, "可用命令: #status, #help");
    }
     
}
