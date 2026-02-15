using System.Reflection;
using System.Runtime.Loader;

namespace QBotSharp.Utils;


public class DllLoadContext(string pluginPath) : AssemblyLoadContext(isCollectible: true)
{
    private readonly AssemblyDependencyResolver _resolver = new(pluginPath);
    protected override Assembly? Load(AssemblyName assemblyName)
    {
        // 查找插件自己的依赖 DLL
        var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        return assemblyPath != null ? LoadFromAssemblyPath(assemblyPath) : null;
    }
}