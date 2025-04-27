using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Domain.Interfaces;

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

    /// <summary>
    /// Sets the default provider
    /// </summary>
    /// <typeparam name="TProvider">The provider type to set as default</typeparam>
    /// <returns>The builder for chaining</returns>
    IContentBuilder SetDefaultProvider<TProvider>()
        where TProvider : class, IContentProvider;

    /// <summary>
    /// Sets the default provider by ID
    /// </summary>
    /// <param name="providerId">The ID of the provider to set as default</param>
    /// <returns>The builder for chaining</returns>
    IContentBuilder SetDefaultProvider(string providerId);
}