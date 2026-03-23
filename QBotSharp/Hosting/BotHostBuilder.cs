namespace QBotSharp.Hosting;

public class BotHostBuilder
{
    private BotHostBuilder() { }

    public static BotHostBuilder CreateDefault()
    {
        return new BotHostBuilder();
    }

    public BotHostBuilder UsePluginSystem()
    {
        return this;
    }

    public Task<BotHost> BuildAsync()
    {
        return Task.FromResult(new BotHost());
    }
}
