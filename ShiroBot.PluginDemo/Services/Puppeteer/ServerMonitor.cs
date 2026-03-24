using Swan.Logging;

namespace ShiroBot.PluginDemo.Services.Puppeteer;

public class ServerMonitor
{
    public async Task MonitorAsync()
    {
        while (true)
        {
            try
            {
                var browser = await BrowserManager.GetBrowserAsync();
                
                // 这里可以考虑直接调用一个重启方法，或者让 GetBrowserAsync 逻辑处理重启
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now}] 监控异常: {ex.Message}");
            }

            await Task.Delay(30000); // 每30秒检查一次
        }
    }
}