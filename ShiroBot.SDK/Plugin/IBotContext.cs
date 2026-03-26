using ShiroBot.SDK.Config;

namespace ShiroBot.SDK.Plugin;

public interface IBotContext
{
    public IFileContext File { get; }
    public IFriendContext Friend { get; }
    public IGroupContext Group { get; }
    public IMessageContext Message { get;  }  
    public ISystemContext System { get; }
    public IConfigContext Config { get; }
    public IReadOnlyList<long> OwnerList { get; }
    public IReadOnlyList<long> AdminList { get; }

    public bool IsOwner(long userId) => OwnerList.Contains(userId);

    public bool IsAdmin(long userId) => IsOwner(userId) || AdminList.Contains(userId);
}
