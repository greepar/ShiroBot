namespace QBotSharp.PluginDemo;

public sealed class DemoPluginConfig
{
    public long[] AllowGroups { get; set; } = [622603336, 742274811];
    public bool SendStartupHello { get; set; } = true;
    public bool EnableHotReload { get; set; } = true;
}
