using Milky.Net.Model;

namespace QBotSharp.SDK.Adapter;

public interface IEventService
{
    event Func<GroupIncomingMessage,Task> GroupMessageReceived;
}