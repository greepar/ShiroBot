namespace ShiroBot.Core;

public class DllLoader<T>
    where T : class
{
    private DllLoadContext? _alc;

    public T Load(string dllPath)
    {
        _alc = new DllLoadContext(dllPath);

        var assembly = _alc.LoadFromAssemblyPath(dllPath);

        var candidateTypes = assembly.GetTypes()
            .Where(t =>
                typeof(T).IsAssignableFrom(t) &&
                t is { IsAbstract: false, IsInterface: false })
            .ToList();

        if (candidateTypes.Count == 0)
            throw new Exception($"No {typeof(T).Name} found in DLL");

        // 优先查找标记了 PrimaryAdapter 特性的类型
        var primaryType = candidateTypes
            .FirstOrDefault(t => t.GetCustomAttributes(false)
                .Any(attr => attr.GetType().Name == "BotAdapterAttribute"));

        var type = primaryType ?? candidateTypes.First();

        return Activator.CreateInstance(type) as T ?? throw new InvalidOperationException("Failed to create dllInstance");
    }

    public void Unload()
    {
        _alc?.Unload();
        _alc = null;

        GC.Collect();
        GC.WaitForPendingFinalizers();
    }
}
