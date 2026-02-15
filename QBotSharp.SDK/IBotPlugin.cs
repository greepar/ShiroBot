using QBotSharp.SDK.Plugin;

namespace QBotSharp.SDK;

public interface IBotPlugin
{
    string Name { get; }

    Task OnLoad(IBotContext context);
    Task OnUnload();
}