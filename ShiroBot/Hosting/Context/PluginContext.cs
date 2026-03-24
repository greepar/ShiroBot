using ShiroBot.SDK;
using ShiroBot.SDK.Config;
using ShiroBot.SDK.Core;
using ShiroBot.SDK.Plugin;

namespace ShiroBot.Hosting.Context;

internal sealed class PluginContext : IBotContext
{
    public IFileContext File => _botContext.File;
    public IFriendContext Friend => _botContext.Friend;
    public IGroupContext Group => _botContext.Group;
    public IMessageContext Message => _botContext.Message;
    public ISystemContext System => _botContext.System;
    public IEventContext Event { get; }
    public IConfigContext Config { get; }
    public IConsoleLogger Logger { get; }

    private readonly BotContext _botContext;

    public PluginContext(
        BotContext botContext,
        IBotAdapter adapter,
        string pluginName,
        string? pluginDirectory = null,
        Func<long, bool>? groupRouteFilter = null)
    {
        _botContext = botContext;
        Logger = new ConsoleLogger($"[Plugin:{pluginName}]");
        Event = new EventContext(adapter.Event, Logger, groupRouteFilter);
        Config = ConfigContext.ForPlugin(
            pluginDirectory ?? Path.Combine(AppContext.BaseDirectory, "plugins", pluginName));
    }
}
