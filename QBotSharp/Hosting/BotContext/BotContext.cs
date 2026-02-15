using QBotSharp.SDK;
using QBotSharp.SDK.Adapter;
using QBotSharp.SDK.Plugin;

namespace QBotSharp.Hosting.BotContext;

internal class BotContext(IBotAdapter adapter) : IBotContext
{
    public IFileContext File { get;} = new FileContext(adapter.File);
    public IFriendContext Friend { get;} = new FriendContext(adapter.Friend);
    public IGroupContext Group { get;} = new GroupContext(adapter.Group);
    public IMessageContext Message { get;} = new MessageContext(adapter.Message);
    public ISystemContext System { get;} = new SystemContext(adapter.System);
    public IEventContext Event { get;} = new EventContext(adapter.Event);
}