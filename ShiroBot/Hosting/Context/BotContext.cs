using ShiroBot.SDK;
using ShiroBot.SDK.Core;
using ShiroBot.SDK.Plugin;

namespace ShiroBot.Hosting.Context;

internal sealed class BotContext(IBotAdapter adapter, IReadOnlyList<long> ownerList, IReadOnlyList<long> adminList)
{
    public IFileContext File { get; } = new FileContext(adapter.File);
    public IFriendContext Friend { get; } = new FriendContext(adapter.Friend);
    public IGroupContext Group { get; } = new GroupContext(adapter.Group);
    public IMessageContext Message { get; } = new MessageContext(adapter.Message);
    public ISystemContext System { get; } = new SystemContext(adapter.System);
    public IReadOnlyList<long> OwnerList { get; } = ownerList;
    public IReadOnlyList<long> AdminList { get; } = adminList;
}
