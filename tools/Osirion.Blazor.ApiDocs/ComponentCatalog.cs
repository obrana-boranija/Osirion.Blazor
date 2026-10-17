using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.ApiDocs;

/// <summary>Finds the Osirion components whose parameter tables can be generated.</summary>
internal static class ComponentCatalog
{
    /// <summary>
    /// Loads every <c>Osirion.Blazor*.dll</c> in <paramref name="directory"/> and returns its public, non-abstract
    /// components by name; the first assembly, in name order, wins a name both define.
    /// </summary>
    /// <param name="directory">The folder with the assemblies and their XML documentation, such as the tool's own.</param>
    /// <returns>The components by type name.</returns>
    public static Dictionary<string, Type> Load(string directory)
    {
        var components = new Dictionary<string, Type>(StringComparer.Ordinal);
        foreach (var path in Directory.EnumerateFiles(directory, "Osirion.Blazor*.dll").Order(StringComparer.Ordinal))
        {
            var assembly = Assembly.LoadFrom(path);
            foreach (var type in assembly.GetExportedTypes().Where(type => typeof(IComponent).IsAssignableFrom(type) && !type.IsAbstract))
            {
                components.TryAdd(type.Name, type);
            }
        }

        return components;
    }
}
