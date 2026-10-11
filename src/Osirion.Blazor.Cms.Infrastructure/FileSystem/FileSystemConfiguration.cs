using Microsoft.Extensions.Configuration;
using Osirion.Blazor.Cms.Domain.Options;

namespace Osirion.Blazor.Cms.Infrastructure.FileSystem;

/// <summary>
/// Finds the configuration section of the FileSystem provider.
/// </summary>
/// <remarks>
/// <c>AddOsirion(configuration)</c> enables the provider when <see cref="FileSystemOptions.WebSection"/> exists, but the
/// options used to be bound only from <see cref="FileSystemOptions.Section"/>, so a host had to repeat its settings
/// under both keys. The web section is now read first and the older section is kept as a fallback.
/// </remarks>
internal static class FileSystemConfiguration
{
    /// <summary>
    /// Returns <see cref="FileSystemOptions.WebSection"/> when it exists, otherwise <see cref="FileSystemOptions.Section"/>.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The section to bind <see cref="FileSystemOptions"/> from.</returns>
    internal static IConfigurationSection GetSection(IConfiguration configuration)
    {
        var webSection = configuration.GetSection(FileSystemOptions.WebSection);
        return webSection.Exists() ? webSection : configuration.GetSection(FileSystemOptions.Section);
    }
}
