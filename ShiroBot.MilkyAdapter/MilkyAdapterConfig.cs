namespace ShiroBot.MilkyAdapter;

public sealed class MilkyAdapterConfig
{
    public string BaseUrl { get; set; } = "http://localhost:3010/";
    public string AccessToken { get; set; } = "nmsl.233";
    public bool EnableHotReload { get; set; } = false;
}
