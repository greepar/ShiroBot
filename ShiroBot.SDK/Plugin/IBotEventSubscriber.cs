using ShiroBot.Model.Common;

namespace ShiroBot.SDK.Plugin;

public interface IBotEventSubscriber
{
    Task OnEventAsync(Event e);
}
