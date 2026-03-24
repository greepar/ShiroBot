using ShiroBot.Model.Common;
using ShiroBot.SDK;
using ShiroBot.SDK.Abstractions;
using ShiroBot.SDK.Plugin;

namespace ShiroBot.DemoPlugin;

public sealed class DemoPlugin : PluginBase
{
    private DemoPluginConfig _config = new();
    private IDisposable? _configWatcher;

    public override string Name => "DemoPlugin";
    public override BotComponentMetadata Metadata { get; } = new()
    {
        Name = "DemoPlugin",
        Version = "1.0.0",
        Description = "标准示例插件"
    };

    protected override Task OnLoadAsync(IBotContext context)
    {
        _config = context.Config.Load<DemoPluginConfig>();
        context.Config.Save(_config);

        if (_config.EnableHotReload)
        {
            _configWatcher = context.Config.Watch<DemoPluginConfig>(updated =>
            {
                _config = updated;
                BotLog.Info($"插件 {Name} 配置已热重载: {context.Config.ConfigPath}");
            });
        }

        if (_config.SendStartupHello)
        {
            BotLog.Info("标准示例插件已加载。");
        }

        FriendCommands.Map("#help", HandleHelpAsync);
        FriendCommands.Map("#ping", HandlePingAsync);
        GroupCommands.Map("#help", HandleHelpAsync);
        GroupCommands.Map("#ping", HandlePingAsync);

        return Task.CompletedTask;
    }

    protected override Task OnUnloadAsync()
    {
        _configWatcher?.Dispose();
        _configWatcher = null;
        BotLog.Info("标准示例插件已卸载。");
        return Task.CompletedTask;
    }

    private Task HandleHelpAsync(FriendIncomingMessage message) =>
        Context.Message.ReplyTextAsync(message, "可用命令: #help, #ping");

    private Task HandlePingAsync(FriendIncomingMessage message) =>
        Context.Message.ReplyTextAsync(message, "pong");

    private Task HandleHelpAsync(GroupIncomingMessage message) =>
        Context.Message.ReplyTextAsync(message, "可用命令: #help, #ping");

    private Task HandlePingAsync(GroupIncomingMessage message) =>
        Context.Message.ReplyTextAsync(message, "pong");
}
