using QBotSharp.Utils;

namespace QBotSharp;

public class BotHostBuilder
{
    private readonly ConfigManager? _configManager;
    
    private Type? _adapterType;
    private bool _usePluginSystem;

    private BotHostBuilder() { }

    public static BotHostBuilder CreateDefault()
    {
        return new BotHostBuilder();
    }
    

    // public BotHostBuilder UseAdapter<T>()
    //     where T : class, IConnection
    // {
    //     _adapterType = typeof(T);
    //     return this;
    // }

    public BotHostBuilder UsePluginSystem()
    {
        _usePluginSystem = true;
        return this;
    }

    public async Task<BotHost> BuildAsync()
    {
        
        // 0 加载配置
        var configManager = _configManager ?? new ConfigManager();
        // 1 创建 Adapter
        // IConnection? adapter = null;
        // if (_adapterType != null)
        // {
        //     adapter = (IConnection)Activator.CreateInstance(_adapterType)!;
        // }

        // 2 创建 BotHost
        // var host = new BotHost(
        //     configManager,
        //     adapter,
        //     _usePluginSystem
        // );
        
        return new BotHost();
    }
    
}
