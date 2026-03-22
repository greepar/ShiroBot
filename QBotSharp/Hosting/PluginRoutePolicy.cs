namespace QBotSharp.Hosting;

public sealed class PluginRoutePolicy
{
    public PluginRouteRule Default { get; set; } = new()
    {
        Mode = "blacklist",
        Groups = []
    };
    public Dictionary<string, PluginRouteRule> Plugins { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public bool AllowsGroup(string pluginName, long groupId)
    {
        if (!Plugins.TryGetValue(pluginName, out var rule))
        {
            return Default.IsMatch(groupId);
        }

        return rule.IsMatch(groupId);
    }
}
