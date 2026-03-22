using CH = QBotSharp.Core.ConsoleHelper;

namespace QBotSharp.Hosting;

public class BotHost
{
    public async Task RunAsync()
    {
        
        CH.Log("BotHost 启动中...");

        // 1. 启动连接
        // 2. 启动 HTTP
        // 3. 加载插件
        // 4. 阻塞运行

        await Task.Delay(1000);
    }

    public async Task StopAsync()
    {
        CH.Log("BotHost 正在关闭...");
        await Task.CompletedTask;
    }
}