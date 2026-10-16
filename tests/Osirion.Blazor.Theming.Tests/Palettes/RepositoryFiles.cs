namespace Osirion.Blazor.Theming.Tests.Palettes;

/// <summary>Finds files of the Osirion.Blazor.Theming source tree from the test's output folder.</summary>
internal static class RepositoryFiles
{
    private static readonly Lazy<string> Root = new(() =>
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Osirion.Blazor.slnx")))
            {
                return directory.FullName;
            }
        }

        throw new InvalidOperationException("Osirion.Blazor.slnx not found above " + AppContext.BaseDirectory);
    });

    public static string Theming(params string[] parts) =>
        Path.Combine([Root.Value, "src", "Osirion.Blazor.Theming", .. parts]);

    public static string ThemingCss(string file) => Theming("wwwroot", "css", file);
}
