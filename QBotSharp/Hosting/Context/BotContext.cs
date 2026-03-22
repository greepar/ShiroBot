using QBotSharp.SDK;
using QBotSharp.SDK.Plugin;

namespace QBotSharp.Hosting.Context;

internal class BotContext : IBotContext
{
    public IFileContext File { get; }
    public IFriendContext Friend { get; }
    public IGroupContext Group { get; }
    public IMessageContext Message { get; }
    public ISystemContext System { get; }
    public IEventContext Event { get; }
    public IPluginConfigContext Config { get; }
    public IConsoleLogger Logger { get; }

    public BotContext(
        IBotAdapter adapter,
        string? pluginName = null,
        string? pluginDirectory = null,
        Func<long, bool>? groupRouteFilter = null)
    {
        File = new FileContext(adapter.File);
        Friend = new FriendContext(adapter.Friend);
        Group = new GroupContext(adapter.Group);
        Message = new MessageContext(adapter.Message);
        System = new SystemContext(adapter.System);
        Logger = new ConsoleLogger($"[Plugin:{pluginName ?? "UnknownPlugin"}]");
        Event = new EventContext(adapter.Event, Logger, groupRouteFilter);
        Config = new PluginConfigContext(
            pluginDirectory ?? Path.Combine(AppContext.BaseDirectory, "plugins", pluginName ?? "UnknownPlugin"));
    }
}
