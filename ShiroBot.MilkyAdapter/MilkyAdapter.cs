using ShiroBot.MilkyAdapter.AdapterImpl;
using ShiroBot.MilkyAdapter.Milky;
using ShiroBot.SDK;
using ShiroBot.SDK.Adapter;
using ShiroBot.SDK.Config;
using ShiroBot.SDK.Plugin;

namespace ShiroBot.MilkyAdapter;

public class MilkyAdapter : IBotAdapter
{
    private IDisposable? _configWatcher;
    public string Name => "milky";
    public BotComponentMetadata Metadata { get; } = new()
    {
        Name = "ShiroBot.MilkyAdapter",
        Version = "1.0.0",
        Description = "Milky 适配器"
    };

    public IFileService File { get; } = new FileService();
    public IFriendService Friend { get; } = new FriendService();
    public IGroupService Group { get; } = new GroupService();
    public IMessageService Message { get; } = new MessageService();
    public ISystemService System { get; } = new SystemService();
    public IEventService Event { get; } = new EventService();
    public IConfigContext Config { get; set; } = null!;
    public IConsoleLogger Logger { get; set; } = null!;

    private CancellationTokenSource? _sseTokenSource;

    public async Task StartAsync()
    {
        var config = Config.Load<MilkyAdapterConfig>();
        Config.Save(config);

        if (config.EnableHotReload)
        {
            _configWatcher = Config.Watch<MilkyAdapterConfig>(updated =>
            {
                Logger.Info($"MilkyAdapter 配置已变更: {Config.ConfigPath}");
                Logger.Warning("BaseUrl/AccessToken 的热更新当前只通知，不自动重连。");
            });
        }

        MilkyClientManager.Initialize(config.BaseUrl, config.AccessToken);
        var milky = MilkyClientManager.Instance;
        Logger.Info("开始连接 Milky...");
        var loginInfo = await milky.System.GetLoginInfoAsync();
        var result = await milky.System.GetImplInfoAsync();
        Logger.Success($"Milky 登录信息获取成功 - Nickname: {loginInfo.Nickname},Milky Impl: {result.ImplName} {result.ImplVersion}");
        Logger.Success("Milky 登录完成。");

        _sseTokenSource = new CancellationTokenSource();
        _ = Task.Run(async () =>
        {
            var retryCount = 0;

            while (!_sseTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    retryCount++;
                    Logger.Info($"正在尝试连接 SSE 事件流，第 {retryCount} 次。");
                    await milky.ReceivingEventUsingSSEAsync(_sseTokenSource.Token);
                }
                catch (TaskCanceledException)
                {
                    Logger.Warning("SSE 事件接收已取消。");
                    break;
                }
                catch (Exception ex)
                {
                    Logger.Error($"SSE 事件接收异常: {ex.GetType().Name}: {ex.Message}");

                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(5), _sseTokenSource.Token);
                    }
                    catch (TaskCanceledException)
                    {
                        break;
                    }
                }
            }
        });

    }

    public Task StopAsync()
    {
        _configWatcher?.Dispose();
        _configWatcher = null;
        _sseTokenSource?.Cancel();
        _sseTokenSource?.Dispose();
        _sseTokenSource = null;
        return Task.CompletedTask;
    }
}
