using QBotSharp.SDK.Adapter;
using QBotSharp.SDK.Plugin;

namespace QBotSharp.SDK;

public interface IBotAdapter
{
    string Name { get; }
    BotComponentMetadata Metadata { get; }
    IAdapterConfigContext Config { get; set; }
    IConsoleLogger Logger { get; set; }
    public IFileService File { get; }
    public IFriendService Friend { get; }
    public IGroupService Group { get; }
    public IMessageService Message { get; }
    public ISystemService System { get; }
    public IEventService Event { get; }

    Task StartAsync();
    Task StopAsync() => Task.CompletedTask;
}
