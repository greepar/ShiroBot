using Milky.Net.Model;
using QBotSharp.SDK;
using QBotSharp.SDK.Adapter;
using QBotSharp.SDK.Plugin;

namespace QBotSharp.Hosting.BotContext;

public class EventContext(IEventService eventService) : IEventContext
{
    public event Func<GroupIncomingMessage, Task> GroupMessageReceived
    {
        add => eventService.GroupMessageReceived += value;
        remove => eventService.GroupMessageReceived -= value;
    }}