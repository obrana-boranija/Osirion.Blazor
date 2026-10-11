using System.Reflection;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.ApiDocs;

// Regenerates the parameter tables in the site content from the components themselves.
//   dotnet run --project tools/Osirion.Blazor.ApiDocs -- <content-folder>          rewrites stale tables
//   dotnet run --project tools/Osirion.Blazor.ApiDocs -- <content-folder> --check  fails when a table is stale
if (args.Length == 0 || args[0] is "-h" or "--help")
{
    Console.WriteLine("Usage: Osirion.Blazor.ApiDocs <content-folder> [--check]");
    return 2;
}

var root = args[0];
var check = args.Contains("--check", StringComparer.Ordinal);
if (!Directory.Exists(root))
{
    Console.Error.WriteLine($"Folder not found: {root}");
    return 2;
}

// The meta package reference copies every module's assembly and XML docs next to this tool.
var components = new Dictionary<string, Type>(StringComparer.Ordinal);
foreach (var path in Directory.EnumerateFiles(AppContext.BaseDirectory, "Osirion.Blazor*.dll").Order(StringComparer.Ordinal))
{
    var assembly = Assembly.LoadFrom(path);
    foreach (var type in assembly.GetExportedTypes().Where(type => typeof(IComponent).IsAssignableFrom(type) && !type.IsAbstract))
    {
        components.TryAdd(type.Name, type);
    }
}

var tables = new ParameterTable();
var stale = 0;
var failed = false;
foreach (var file in Directory.EnumerateFiles(root, "*.md", SearchOption.AllDirectories))
{
    var text = await File.ReadAllTextAsync(file);
    var updated = ContentUpdater.Update(
        text,
        name => components.TryGetValue(name, out var type) ? ParameterTable.ToMarkdown(tables.GetRows(type)) : null,
        out var unknown);

    foreach (var name in unknown)
    {
        Console.Error.WriteLine($"{file}: no component named {name}");
        failed = true;
    }

    if (updated == text)
    {
        continue;
    }

    stale++;
    if (check)
    {
        Console.Error.WriteLine($"{file}: parameter table is out of date");
    }
    else
    {
        await File.WriteAllTextAsync(file, updated);
        Console.WriteLine($"updated {file}");
    }
}

Console.WriteLine($"{components.Count} components, {stale} file(s) {(check ? "out of date" : "updated")}");
return failed || (check && stale > 0) ? 1 : 0;
