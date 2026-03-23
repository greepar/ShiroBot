namespace QBotSharp.SDK.Config;

public interface IConfigContext
{
    string ConfigPath { get; }
    T Load<T>() where T : class, new();
    void Save<T>(T config) where T : class;
    IDisposable Watch<T>(Action<T> onChanged, int debounceMs = 500) where T : class, new();
}
