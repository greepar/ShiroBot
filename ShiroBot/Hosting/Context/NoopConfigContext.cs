using ShiroBot.SDK.Config;

namespace ShiroBot.Hosting.Context;

internal sealed class NoopConfigContext : IConfigContext
{
    public string ConfigPath => string.Empty;

    public T Load<T>() where T : class, new() => new();

    public void Save<T>(T config) where T : class
    {
    }

    public IDisposable Watch<T>(Action<T> onChanged, int debounceMs = 500) where T : class, new()
        => new NoopDisposable();

    private sealed class NoopDisposable : IDisposable
    {
        public void Dispose()
        {
        }
    }
}
