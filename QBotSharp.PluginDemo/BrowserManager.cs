using PuppeteerSharp;

namespace QBotSharp.PluginDemo;

public static class BrowserManager
{
    private static IBrowser? _sharedBrowser;
    private static readonly SemaphoreSlim Lock = new(1, 1);
    private static CancellationTokenSource? _closeCts;
    
    // 配置：闲置多久后关闭浏览器 (毫秒)
    private const int IdleTimeoutMs = 60000; 
    
    public static async Task<IBrowser> GetBrowserAsync()
    {
        // 1. 进入锁，防止并发导致创建多个浏览器
        await Lock.WaitAsync();
        try
        {
            // 2. 如果有正在进行的“关闭倒计时”，取消它，因为我们现在需要用浏览器
            if (_closeCts != null)
            {
                await _closeCts.CancelAsync();
                _closeCts.Dispose();
                _closeCts = null;
            }

            // 3. 检查当前浏览器是否可用
            ScheduleClose();
            if (_sharedBrowser is { IsConnected: true }) return _sharedBrowser;
            // 首次加载或已断开，需要重新下载(检查)并启动
            // 注意：DownloadAsync 比较耗时，建议在程序启动时全局做一次，或者这里加个标志位只做一次
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync(); 
            _sharedBrowser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Args = ["--no-sandbox", "--disable-setuid-sandbox", "--lang=zh-CN"]
            });
            await (await _sharedBrowser.PagesAsync()).First().CloseAsync();
            return _sharedBrowser;
        }
        finally
        {
            Lock.Release();
        }
    }

    /// <summary>
    /// 任务结束后调用此方法，安排自动关闭
    /// </summary>
    private static void ScheduleClose()
    {
        // 不要 await 这个方法，让它在后台跑
        _ = Task.Run(async () =>
        {
            await Lock.WaitAsync();
            try
            {
                // 如果已经有倒计时在跑，先取消旧的（虽然理论上逻辑走到这不应该有旧的，但为了安全）
                if (_closeCts != null)
                {
                    await _closeCts.CancelAsync();
                    _closeCts = new CancellationTokenSource();
                }
            }
            finally
            {
                Lock.Release();
            }

            if (_closeCts != null)
            {
                var token = _closeCts.Token;

                try
                {
                    // 等待闲置时间
                    await Task.Delay(IdleTimeoutMs, token);

                    // 如果没被取消，说明时间到了，关闭浏览器
                    await Lock.WaitAsync(token);
                    try
                    {
                        if (!token.IsCancellationRequested && _sharedBrowser != null)
                        {
                            Console.WriteLine("浏览器闲置超时，正在关闭释放资源...");
                            await _sharedBrowser.CloseAsync();
                            await _sharedBrowser.DisposeAsync();
                            _sharedBrowser = null;
                        }
                    }
                    finally
                    {
                        Lock.Release();
                    }
                }
                catch (TaskCanceledException)
                {
                    // 被取消了，说明有新请求进来了，保持浏览器开启
                    // Console.WriteLine("浏览器关闭计划被取消，继续复用");
                }
            }
        });
    }
}