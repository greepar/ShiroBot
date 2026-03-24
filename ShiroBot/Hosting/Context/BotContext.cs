using ShiroBot.SDK;
using ShiroBot.SDK.Plugin;

namespace ShiroBot.Hosting.Context;

internal sealed class BotContext(IBotAdapter adapter)
{
    public IFileContext File { get; } = new FileContext(adapter.File);
    public IFriendContext Friend { get; } = new FriendContext(adapter.Friend);
    public IGroupContext Group { get; } = new GroupContext(adapter.Group);
    public IMessageContext Message { get; } = new MessageContext(adapter.Message);
    public ISystemContext System { get; } = new SystemContext(adapter.System);
}
