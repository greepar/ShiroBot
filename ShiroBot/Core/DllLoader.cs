using System.Runtime.Loader;

namespace ShiroBot.Core;

public class DllLoader<T>
    where T : class
{
    private AssemblyLoadContext? _alc;
    private WeakReference? _alcWeakReference;

    public T Load(string dllPath)
    {
        _alc = new AssemblyLoadContext(dllPath, isCollectible: true);
        _alcWeakReference = new WeakReference(_alc);

        var assembly = _alc.LoadFromAssemblyPath(dllPath);

        var candidateTypes = assembly.GetTypes()
            .Where(t =>
                typeof(T).IsAssignableFrom(t) &&
                t is { IsAbstract: false, IsInterface: false })
            .ToList();

        if (candidateTypes.Count == 0)
            throw new Exception($"No {typeof(T).Name} found in DLL");
        
        var primaryType = candidateTypes
            .FirstOrDefault() ?? candidateTypes.First();
        
        return Activator.CreateInstance(primaryType) as T ?? throw new InvalidOperationException("Failed to create dllInstance");
    }

    public WeakReference? BeginUnload()
    {
        _alc?.Unload();
        _alc = null;

        return _alcWeakReference;
    }

    public static bool WaitForUnload(WeakReference? alcWeakReference, int maxAttempts = 50, int delayMs = 100)
    {
        if (alcWeakReference is null)
        {
            return true;
        }

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            if (!alcWeakReference.IsAlive)
            {
                return true;
            }

            Thread.Sleep(delayMs);
        }

        return !alcWeakReference.IsAlive;
    }

    private bool UnloadAndWait(int maxAttempts = 50, int delayMs = 100)
    {
        var alcWeakReference = BeginUnload();
        var unloaded = WaitForUnload(alcWeakReference, maxAttempts, delayMs);
        if (unloaded)
        {
            _alcWeakReference = null;
        }

        return unloaded;
    }

    public void Unload()
    {
        _ = UnloadAndWait();
    }
}
