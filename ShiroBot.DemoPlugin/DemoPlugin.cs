using ShiroBot.Model.Common;
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

    protected override async Task LoadAsync()
    {
        var config = Context.Config.Load<DemoPluginConfig>();
        if (config.SendStartupHelloToOwner)
        {
            foreach (var id in Context.OwnerList)
            {
                await Context.Message.SendPrivateMessageAsync(id, "ShiroBot已启动.！");
            }
        }
        FriendCommands.MapExact("#help", HandleFriendHelpAsync);
        FriendCommands.MapExact("#ping", HandleFriendPingAsync);
        FriendCommands.MapPrefix("#echo", HandleFriendEchoAsync);
        FriendCommands.MapPrefix("#server", async message =>
        {
            BotLog.Error("这是一个错误日志示例");
            await Context.Message.ReplyAsync(message, "服务器信息: ShiroBot Demo Server v1.0");
        });
        GroupCommands.Map("#help", HandleGroupHelpAsync);
        GroupCommands.Map("#ping", HandleGroupPingAsync);
        GroupCommands.Map("#echo", HandleGroupEchoAsync);
        var loginInfo = await Context.System.GetLoginInfoAsync();
        BotLog.Info($"插件上下文已就绪: {loginInfo.Nickname}");
        BotLog.Info("标准示例插件已加载。");
    }

    protected override Task OnUnloadAsync()
    {
        BotLog.Info("标准示例插件已卸载。");
        return Task.CompletedTask;
    }

    private Task HandleFriendHelpAsync(FriendIncomingMessage message) =>
        Context.Message.ReplyAsync(message, "可用命令: #help, #ping, #echo <内容>");

    private Task HandleFriendPingAsync(FriendIncomingMessage message) =>
        Context.Message.ReplyAsync(message, "pong");

    private Task HandleFriendEchoAsync(FriendIncomingMessage message)
    {
        var content = ExtractEchoContent(message.GetPlainText());
        return Context.Message.ReplyAsync(message, $"你说了: {content}");
    }

    private Task HandleGroupHelpAsync(GroupIncomingMessage message) =>
        Context.Message.ReplyAsync(message, "可用命令: #help, #ping, #echo <内容>");

    private Task HandleGroupPingAsync(GroupIncomingMessage message) =>
        Context.Message.ReplyAsync(message, "pong");

    private Task HandleGroupEchoAsync(GroupIncomingMessage message)
    {
        var content = ExtractEchoContent(message.GetPlainText());
        return Context.Message.ReplyAsync(message, $"你说了: {content}");
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
