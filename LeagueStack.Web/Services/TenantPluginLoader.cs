using LeagueStack.Abstractions;
using System.Reflection;

namespace LeagueStack.Web.Services;

public static class TenantPluginLoader
{
    public static IReadOnlyList<ITenantPlugin> DiscoverPlugins()
    {
        var pluginType = typeof(ITenantPlugin);

        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        foreach (var dllFile in Directory.GetFiles(baseDirectory, searchPattern: "LeagueStack.Tenant*.dll"))
        {
            var name = AssemblyName.GetAssemblyName(dllFile).Name;

            if (AppDomain.CurrentDomain.GetAssemblies().All(assembly => assembly.GetName().Name != name))
            {
                Assembly.LoadFrom(dllFile);
            }
        }

        return [.. AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly =>
            {
                try
                {
                    return assembly.GetTypes();
                }
                catch
                {
                    return [];
                }
            })
            .Where(type => pluginType.IsAssignableFrom(type) && type is { IsInterface: false, IsAbstract: false })
            .Select(type => (ITenantPlugin)Activator.CreateInstance(type)!)];
    }
}