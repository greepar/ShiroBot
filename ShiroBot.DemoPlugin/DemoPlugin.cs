using ShiroBot.Model.Common;
using ShiroBot.SDK;
using ShiroBot.SDK.Abstractions;
using ShiroBot.SDK.Core;
using ShiroBot.SDK.Plugin;

namespace ShiroBot.DemoPlugin;

public sealed class DemoPlugin : PluginBase
{
    public override string Name => "DemoPlugin";
    public override BotComponentMetadata Metadata { get; } = new()
    {
        Name = "标准示例插件",
        Version = "1.0.0",
        Description = "这是一个 ShiroBot 插件开发的标准示例，展示了插件的基本结构和功能。"
    };

    protected override async Task OnLoadAsync(IBotContext context)
    {
        FriendCommands.MapExact("#help", HandleFriendHelpAsync);
        FriendCommands.MapExact("#ping", HandleFriendPingAsync);
        FriendCommands.MapPrefix("#echo", HandleFriendEchoAsync);
        // GroupCommands.Map("#help", HandleGroupHelpAsync);
        // GroupCommands.Map("#ping", HandleGroupPingAsync);
        // GroupCommands.Map("#echo", HandleGroupEchoAsync);
        var loginInfo = await context.System.GetLoginInfoAsync();
        BotLog.Info($"插件上下文已就绪: {loginInfo.Nickname}");
        BotLog.Info("标准示例插件已加载。");
    }

    protected override Task OnUnloadAsync()
    {
        BotLog.Info("标准示例插件已卸载。");
        return Task.CompletedTask;
    }

    private Task HandleFriendHelpAsync(FriendIncomingMessage message) =>
        Context.Message.ReplyTextAsync(message, "可用命令: #help, #ping, #echo <内容>");

    private Task HandleFriendPingAsync(FriendIncomingMessage message) =>
        Context.Message.ReplyTextAsync(message, "pong");

    private Task HandleFriendEchoAsync(FriendIncomingMessage message)
    {
        var content = ExtractEchoContent(message.GetPlainText());
        return Context.Message.ReplyTextAsync(message, $"你说了: {content}");
    }

    private Task HandleGroupHelpAsync(GroupIncomingMessage message) =>
        Context.Message.ReplyTextAsync(message, "可用命令: #help, #ping, #echo <内容>");

    private Task HandleGroupPingAsync(GroupIncomingMessage message) =>
        Context.Message.ReplyTextAsync(message, "pong");

    private Task HandleGroupEchoAsync(GroupIncomingMessage message)
    {
        var content = ExtractEchoContent(message.GetPlainText());
        return Context.Message.ReplyTextAsync(message, $"你说了: {content}");
    }

    private static string ExtractEchoContent(string text)
    {
        var trimmed = text.Trim();
        if (trimmed.Length <= "#echo".Length)
        {
            return string.Empty;
        }

        return trimmed["#echo".Length..].TrimStart();
    }
}
