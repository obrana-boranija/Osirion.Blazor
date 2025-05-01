using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Osirion.Blazor.Cms.Domain.Interfaces;

/// <summary>
/// Builder interface for configuring content providers
/// </summary>
public interface IContentBuilder : ICmsBuilder
{
    /// <summary>
    /// Adds a GitHub content provider
    /// </summary>
    /// <param name="configure">Action to configure the GitHub provider options</param>
    /// <returns>The builder for chaining</returns>
    IContentBuilder AddGitHub(Action<GitHubOptions>? configure = null);

    /// <summary>
    /// Adds a file system content provider
    /// </summary>
    /// <param name="configure">Action to configure the file system provider options</param>
    /// <returns>The builder for chaining</returns>
    IContentBuilder AddFileSystem(Action<FileSystemOptions>? configure = null);

    /// <summary>
    /// Adds a custom content provider
    /// </summary>
    /// <typeparam name="TProvider">The provider type implementing IContentProvider</typeparam>
    /// <param name="configure">Action to configure the provider instance</param>
    /// <returns>The builder for chaining</returns>
    IContentBuilder AddProvider<TProvider>(Action<TProvider>? configure = null)
        where TProvider : class, IContentProvider;
}