namespace QBotSharp.Hosting;

public sealed class PluginRouteRule
{
    public string Mode { get; set; } = "whitelist";
    public long[] Groups { get; set; } = [];

    public bool IsMatch(long groupId)
    {
        var contains = Groups.Contains(groupId);
        return NormalizeMode(Mode) switch
        {
            "blacklist" => !contains,
            _ => contains
        };
    }

    private static string NormalizeMode(string? mode)
    {
        return string.IsNullOrWhiteSpace(mode) ? "whitelist" : mode.ToLowerInvariant();
    }
}
