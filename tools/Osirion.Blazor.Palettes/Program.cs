using System.Text;
using Osirion.Blazor.Palettes;

// Writes src/Osirion.Blazor.Theming/wwwroot/css/palettes.css and palettes.min.css from PaletteDefinitions.
// Usage: dotnet run --project tools/Osirion.Blazor.Palettes [-- <repository root>]
var root = args.Length > 0 ? args[0] : FindRepositoryRoot(AppContext.BaseDirectory);
var folder = Path.Combine(root, "src", "Osirion.Blazor.Theming", "wwwroot", "css");

var adjustments = new List<string>();
var (css, minified) = PaletteStylesheet.Generate(PaletteStylesheet.BuildVariants(adjustments));

var utf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
File.WriteAllText(Path.Combine(folder, "palettes.css"), css, utf8);
File.WriteAllText(Path.Combine(folder, "palettes.min.css"), minified, utf8);

Console.WriteLine($"Wrote palettes.css and palettes.min.css to {folder}");
Console.WriteLine(adjustments.Count == 0 ? "No color needed a contrast adjustment." : "Adjusted for contrast:");
adjustments.ForEach(adjustment => Console.WriteLine("  " + adjustment));
return 0;

static string FindRepositoryRoot(string start)
{
    for (var directory = new DirectoryInfo(start); directory is not null; directory = directory.Parent)
    {
        if (File.Exists(Path.Combine(directory.FullName, "Osirion.Blazor.slnx")))
        {
            return directory.FullName;
        }
    }

    throw new InvalidOperationException("Osirion.Blazor.slnx not found above " + start + "; pass the repository root.");
}
