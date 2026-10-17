using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Osirion.Blazor.Examples.Tests.Site;

/// <summary>
/// Hosts the Bootstrap example in memory in Development, reading the repository's content folder by its full path:
/// the site's own setting is relative to the working directory, which a test run does not share.
/// </summary>
public sealed class SiteFactory : WebApplicationFactory<Program>
{
    /// <inheritdoc/>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var root = RepositoryRoot();
        builder.UseEnvironment("Development");
        builder.UseContentRoot(Path.Combine(root, "examples", "Osirion.Blazor.Example.Bootstrap"));
        builder.UseSetting("Osirion:Cms:Web:FileSystem:BasePath", Path.Combine(root, "content"));
        builder.UseSetting("Osirion:Cms:Web:FileSystem:WatchForChanges", "false");
    }

    private static string RepositoryRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Osirion.Blazor.slnx")))
            {
                return directory.FullName;
            }
        }

        throw new InvalidOperationException("Osirion.Blazor.slnx not found above " + AppContext.BaseDirectory);
    }
}
