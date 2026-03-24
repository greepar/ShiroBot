namespace ShiroBot.SDK.Plugin;

public sealed class CommandRouter<TMessage>(StringComparison comparison = StringComparison.OrdinalIgnoreCase)
{
    private readonly List<(string Prefix, Func<TMessage, Task> Handler)> _routes = [];

    public void Map(string prefix, Func<TMessage, Task> handler)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
        ArgumentNullException.ThrowIfNull(handler);
        _routes.Add((prefix, handler));
    }

    public async Task<bool> DispatchAsync(string text, TMessage message)
    {
        foreach (var (prefix, handler) in _routes)
        {
            if (!text.StartsWith(prefix, comparison))
            {
                continue;
            }

            await handler(message);
            return true;
        }

        return false;
    }
}
