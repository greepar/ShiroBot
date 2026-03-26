using ShiroBot.DemoAdapter.AdapterImpl;
using ShiroBot.SDK;
using ShiroBot.SDK.Adapter;
using ShiroBot.SDK.Config;
using ShiroBot.SDK.Core;
using ShiroBot.SDK.Plugin;

namespace ShiroBot.DemoAdapter;


public class DemoAdapter : IBotAdapter
{
    public string Name => "demo";
    public BotComponentMetadata Metadata { get; } = new()
    {
        Name = "ShiroBot.DemoAdapter",
        Version = "1.0.0",
        Description = "示例适配器"
    };

    public IFileService File { get; } = new FileService();
    public IFriendService Friend { get; } = new FriendService();
    public IGroupService Group { get; } = new GroupService();
    public IMessageService Message { get; } = new MessageService();
    public ISystemService System { get; } = new SystemService();
    public IEventService Event { get; } = new EventService();
    public IConfigContext Config { get; set; } = null!;
    public IConsoleLogger Logger { get; set; } = null!;

    public async Task StartAsync()
    {
        var config = Config.Load<DemoAdapterConfig>();
        Config.Save(config);
        await Task.Delay(config.StartupDelayMs);
        Logger.Info(config.StartupText);
    }

    public Task StopAsync() => Task.CompletedTask;
}
