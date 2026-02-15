using Milky.Net.Client;
using Milky.Net.Model;
using QBotSharp.MilkyAdapter.Milky;
using QBotSharp.SDK.Adapter;

namespace QBotSharp.MilkyAdapter.AdapterImpl;

public class EventService : IEventService
{
    private static MilkyClient Milky => MilkyClientManager.Instance;

    public event Func<GroupIncomingMessage, Task>? GroupMessageReceived
    {
        add
        {
            Milky.Events.MessageReceive += async (message,e) =>
            {
                if (e is GroupIncomingMessage groupMessage)
                {
                    await value?.Invoke(groupMessage);
                }
            };
        }
  
        remove
            {

            }
    }
}