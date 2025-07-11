﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Infrastructure.Services;

namespace Osirion.Blazor.Cms.Infrastructure.DependencyInjection;

/// <summary>
/// Builder for configuring Osirion CMS services
/// </summary>
public class OsirionCmsBuilder
{
    /// <summary>
    /// Gets the service collection being configured
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Gets the configuration
    /// </summary>
    public IConfiguration Configuration { get; }

    /// <summary>
    /// Initializes a new instance of the OsirionCmsBuilder class
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    public OsirionCmsBuilder(IServiceCollection services, IConfiguration configuration)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Adds all GitHub providers from configuration (multi-provider support)
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public OsirionCmsBuilder AddGitHubProviders()
    {
        Services.AddGitHubProvidersFromConfiguration(Configuration);
        return this;
    }

    /// <summary>
    /// Adds GitHub provider with optional provider name for multi-provider scenarios
    /// </summary>
    /// <param name="providerName">Optional provider name</param>
    /// <param name="configure">Optional delegate to configure GitHub options</param>
    /// <returns>The builder for chaining</returns>
    public OsirionCmsBuilder AddGitHub(string? providerName = null, Action<GitHubOptions>? configure = null)
    {
        Services.AddGitHubContentProvider(Configuration, providerName, configure);
        return this;
    }

    /// <summary>
    /// Adds FileSystem provider
    /// </summary>
    /// <param name="configure">Optional delegate to configure FileSystem options</param>
    /// <returns>The builder for chaining</returns>
    public OsirionCmsBuilder AddFileSystem(Action<FileSystemOptions>? configure = null)
    {
        Services.AddFileSystemContentProvider(Configuration, configure);
        return this;
    }

    /// <summary>
    /// Adds FileSystem provider from configuration
    /// </summary>
    /// <param name="configSection">Optional configuration section name (default: "Osirion:Cms:FileSystem")</param>
    /// <returns>The builder for chaining</returns>
    public OsirionCmsBuilder AddFileSystem(string configSection = FileSystemOptions.Section)
    {
        var section = Configuration.GetSection(configSection);
        if (section.Exists())
        {
            Services.AddFileSystemContentProvider(Configuration);
        }
        return this;
    }

    /// <summary>
    /// Adds custom content provider
    /// </summary>
    /// <typeparam name="TProvider">Type of the provider to add</typeparam>
    /// <param name="configure">Optional delegate to configure the provider</param>
    /// <param name="isDefault">Whether this provider should be the default</param>
    /// <returns>The builder for chaining</returns>
    public OsirionCmsBuilder AddProvider<TProvider>(
        Action<TProvider>? configure = null,
        bool isDefault = false)
        where TProvider : class, IContentProvider
    {
        // Register the provider
        Services.AddCustomContentProvider(configure, isDefault);
        return this;
    }

    /// <summary>
    /// Sets the default provider
    /// </summary>
    /// <param name="providerId">ID of the provider to set as default</param>
    /// <returns>The builder for chaining</returns>
    public OsirionCmsBuilder SetDefaultProvider(string providerId)
    {
        if (string.IsNullOrWhiteSpace(providerId))
            throw new ArgumentException("Provider ID cannot be empty", nameof(providerId));

        // Register a startup hook to set the default provider
        Services.AddSingleton<IDefaultProviderSetter>(
            new DefaultProviderSetter(providerId, true));

        return this;
    }

    /// <summary>
    /// Configures caching for all providers
    /// </summary>
    /// <param name="configure">Delegate to configure cache options</param>
    /// <returns>The builder for chaining</returns>
    public OsirionCmsBuilder ConfigureCache(Action<CacheOptions> configure)
    {
        if (configure is null)
            throw new ArgumentNullException(nameof(configure));

        Services.Configure(configure);
        return this;
    }

    /// <summary>
    /// Auto-registers all providers from configuration
    /// </summary>
    /// <returns>The builder for chaining</returns>
    public OsirionCmsBuilder AddAllProvidersFromConfiguration()
    {
        // Check for GitHub providers in the new multi-provider format
        var githubWebSection = Configuration.GetSection("Osirion:Cms:Web:GitHub");
        var githubAdminSection = Configuration.GetSection("Osirion:Cms:Admin:GitHub");

        if ((githubWebSection.Exists() && githubWebSection.GetChildren().Any()) ||
            (githubAdminSection.Exists() && githubAdminSection.GetChildren().Any()))
        {
            // Use the new multi-provider registration
            AddGitHubProviders();
        }
        else
        {
            // Fallback to legacy single provider
            var cmsSection = Configuration.GetSection("Osirion:Cms");
            var githubSection = cmsSection.GetSection("GitHub");
            if (githubSection.Exists())
            {
                AddGitHub(null); // Pass null for default provider
            }
        }

        // Check for FileSystem provider
        var fileSystemSection = Configuration.GetSection(FileSystemOptions.Section);
        if (fileSystemSection.Exists())
        {
            //AddFileSystem();
        }

        return this;
    }
}