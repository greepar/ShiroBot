
using Milky.Net.Model;
using QBotSharp.MilkyAdapter.AdapterImpl;
using QBotSharp.MilkyAdapter.Milky;
using QBotSharp.SDK;
using QBotSharp.SDK.Adapter;

namespace QBotSharp.MilkyAdapter;

[BotAdapter]
public class MilkyAdapter : IBotAdapter
{
    public string Name => "demo";
    public IFileService File { get; } = new FileService();
    public IFriendService Friend { get; } = new FriendService();
    public IGroupService Group { get; } = new GroupService();
    public IMessageService Message { get; } = new MessageService();
    public ISystemService System { get; } = new SystemService();
    public IEventService Event { get; } = new EventService();

    
    private CancellationTokenSource? _sseTokenSource;
    private bool _sseConnected = false;



    public async Task StartAsync()
    {
        // 获取 MilkyClient 实例
        MilkyClientManager.Initialize("http://localhost:3010/", "nmsl.233");
        var milky = MilkyClientManager.Instance;
        
        Console.WriteLine($"开始.");
        //
        //
        var loginInfo = await milky.System.GetLoginInfoAsync();
        Console.WriteLine($"MilkyImpl LoginService: 登录信息获取成功 - UserId: {loginInfo.Nickname}");
            
        var result = await milky.System.GetImplInfoAsync();
        Console.WriteLine($"MilkyImpl LoginService: Impl Name - {result.ImplName}, Version - {result.ImplVersion}");
        Console.WriteLine("MilkyImpl LoginService: 登录完成！");
        
        Console.WriteLine("发送测试私聊消息...");
        
        //cts
        _sseTokenSource = new CancellationTokenSource();
        // 尝试启动 SSE 事件接收
        _ = Task.Run(async () =>
        {
            var retryCount = 0;
                
            while (!_sseTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    retryCount++;
                    _sseConnected = false; // 重置连接状态
                    Console.WriteLine($"MilkyImpl LoginService: 正在尝试连接 SSE 事件流... (第 {retryCount} 次)");
                        
                    // 开始接收 SSE 事件（这个方法会一直阻塞，连接断开时会抛出异常）
                    await milky.ReceivingEventUsingSSEAsync(_sseTokenSource.Token);
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("MilkyImpl LoginService: SSE 事件接收已手动取消。");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"MilkyImpl LoginService: SSE 事件接收出现异常 - {ex.GetType().Name}: {ex.Message}");
                    Console.WriteLine($"MilkyImpl LoginService: 将在 5 秒后重试...");
                    _sseConnected = false;
                        
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(5), _sseTokenSource.Token);
                    }
                    catch (TaskCanceledException)
                    {
                        Console.WriteLine("MilkyImpl LoginService: 重连等待被取消。");
                        break;
                    }
                }
            }
                
            Console.WriteLine("MilkyImpl LoginService: SSE 事件接收任务已退出。");
        });
        
        var segments = new OutgoingSegment[]
        {
            new TextOutgoingSegment("hello")
        };
        var request = new SendPrivateMessageRequest(1034028486,segments);
        try
        {
            await milky.Message.SendPrivateMessageAsync(request);

        }
        catch (Exception e)
        {
            Console.WriteLine($"发送消息失败: {e.Message}");
            throw;
        }
        
    }
    
}