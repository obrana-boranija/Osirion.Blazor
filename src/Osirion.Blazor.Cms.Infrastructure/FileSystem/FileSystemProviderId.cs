using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Infrastructure.Utilities;

namespace Osirion.Blazor.Cms.Infrastructure.FileSystem;

/// <summary>
/// Derives the provider id the FileSystem provider and its repositories report for their options.
/// </summary>
/// <remarks>
/// The id must be the same in every process and on every server instance, because content items, directories
/// and the provider registry are keyed by it. It is therefore derived from the configured base path with
/// <see cref="IdGenerator"/> rather than <see cref="string.GetHashCode()"/>, which .NET randomizes per process.
/// </remarks>
internal static class FileSystemProviderId
{
    private const string EmptyBasePath = ".";

    /// <summary>
    /// Returns the configured <see cref="ContentProviderOptions.ProviderId"/>, or <c>filesystem-</c> followed by a
    /// stable hash of the base path when none is configured.
    /// </summary>
    /// <param name="options">The FileSystem provider options.</param>
    /// <returns>The effective provider id.</returns>
    internal static string For(FileSystemOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (!string.IsNullOrWhiteSpace(options.ProviderId))
        {
            return options.ProviderId;
        }

        // Windows and Linux spell the same configured path with different separators; both must give one id.
        var basePath = (options.BasePath ?? string.Empty).Replace('\\', '/').TrimEnd('/');

        return $"filesystem-{IdGenerator.GenerateStableId(basePath.Length == 0 ? EmptyBasePath : basePath)}";
    }
}
