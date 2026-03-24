using System.Reflection;
using System.Runtime.Loader;

namespace ShiroBot.Core;


public class DllLoadContext(string pluginPath) : AssemblyLoadContext(isCollectible: true)
{
    private static readonly HashSet<string> SharedAssemblies = new(StringComparer.OrdinalIgnoreCase)
    {
        "ShiroBot.SDK",
        "ShiroBot.Model"
    };

    private readonly AssemblyDependencyResolver _resolver = new(pluginPath);

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        if (SharedAssemblies.Contains(assemblyName.Name ?? string.Empty))
        {
            return null;
        }

        // 查找插件自己的依赖 DLL
        var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        return assemblyPath != null ? LoadFromAssemblyPath(assemblyPath) : null;
    }

    protected override nint LoadUnmanagedDll(string unmanagedDllName)
    {
        var unmanagedDllPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        return unmanagedDllPath != null
            ? LoadUnmanagedDllFromPath(unmanagedDllPath)
            : base.LoadUnmanagedDll(unmanagedDllName);
    }
}
