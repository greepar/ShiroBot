namespace ShiroBot.SDK.Core;

public sealed class BotComponentMetadata
{
    public string Name { get; init; } = string.Empty;
    public string Version { get; init; } = "1.0.0";
    public string? Description { get; init; }
    public bool? IsPluginSingleFile { get; init; } = false;
}
