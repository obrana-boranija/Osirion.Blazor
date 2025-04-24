using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Interfaces;
using Osirion.Blazor.Cms.Options;

namespace Osirion.Blazor.Cms;

/// <summary>
/// Builder interface for configuring content providers
/// </summary>
public interface IContentBuilder
{
    /// <summary>
    /// Gets the service collection being configured
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Adds a GitHub content provider
    /// </summary>
    /// <param name="configure">Action to configure the GitHub provider</param>
    /// <returns>The builder for chaining</returns>
    IContentBuilder AddGitHub(Action<GitHubContentOptions>? configure = null);

    /// <summary>
    /// Adds a file system content provider
    /// </summary>
    /// <param name="configure">Action to configure the file system provider</param>
    /// <returns>The builder for chaining</returns>
    IContentBuilder AddFileSystem(Action<FileSystemContentOptions>? configure = null);

    /// <summary>
    /// Adds a custom content provider
    /// </summary>
    /// <typeparam name="TProvider">The provider type</typeparam>
    /// <param name="configure">Action to configure the provider</param>
    /// <returns>The builder for chaining</returns>
    IContentBuilder AddProvider<TProvider>(Action<TProvider>? configure = null)
        where TProvider : class, IContentProvider;
}