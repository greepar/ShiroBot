namespace QBotSharp.SDK.Plugin;

public interface IBotContext
{
    public IFileContext File { get; }
    public IFriendContext Friend { get; }
    public IGroupContext Group { get; }
    public IMessageContext Message { get;  }  
    public ISystemContext System { get; }
    public IEventContext Event { get; }
}