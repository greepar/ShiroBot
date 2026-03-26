namespace ShiroBot.SDK.Config;

public interface IConfigContext
{
    string ConfigPath { get; }
    T Load<T>() where T : class, new();
    void Save<T>(T config) where T : class;
}
