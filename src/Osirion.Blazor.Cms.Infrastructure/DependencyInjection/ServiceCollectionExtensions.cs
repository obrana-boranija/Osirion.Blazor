﻿using Blazored.LocalStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Infrastructure.Caching;
using Osirion.Blazor.Cms.Infrastructure.Factories;
using Osirion.Blazor.Cms.Infrastructure.FileSystem;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Osirion.Blazor.Cms.Infrastructure.Markdown;
using Osirion.Blazor.Cms.Infrastructure.Providers;
using Osirion.Blazor.Cms.Infrastructure.Services;

namespace Osirion.Blazor.Cms.Infrastructure.DependencyInjection;

/// <summary>
/// Extension methods for configuring Osirion.Blazor.Cms services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds core Osirion.Blazor.Cms services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <param name="configureCms">Optional delegate to configure CMS services</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddCms(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<OsirionCmsBuilder>? configureCms = null)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (configuration is null) throw new ArgumentNullException(nameof(configuration));

        // Register options
        services.Configure<CacheOptions>(configuration.GetSection(CacheOptions.Section));

        // Required services
        services.AddMemoryCache();

        // Register core services (singletons and scoped services)
        services.TryAddSingleton<IMarkdownProcessor, MarkdownProcessor>();
        services.TryAddScoped<IStateStorageService, LocalStorageService>();
        services.TryAddScoped<IContentCacheService, InMemoryContentCacheService>();

        // Register provider registry and manager
        services.TryAddSingleton<IContentProviderRegistry, ContentProviderRegistry>();
        services.TryAddSingleton<IContentProviderManager, ContentProviderManager>();
        services.TryAddSingleton<IContentProviderInitializer, ContentProviderInitializer>();

        // Register CQRS components
        services.AddOsirionCqrs();

        // Register domain events
        services.AddOsirionDomainEvents();

        // Add markdown services
        services.AddMarkdownServices();

        // Add directory services
        services.AddDirectoryServices();

        // Add content services
        services.AddContentServices();

        // Register repositories and unit of work
        services.TryAddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();

        // Register cache decorator factory
        services.TryAddSingleton<CacheDecoratorFactory>();



        // Apply builder configuration if provided
        if (configureCms is not null)
        {
            var builder = new OsirionCmsBuilder(services, configuration);
            configureCms(builder);
        }
        else
        {
            // Auto-register providers based on configuration sections
            AutoRegisterProviders(services, configuration);
        }

        return services;
    }

    /// <summary>
    /// Adds GitHub content provider with support for multiple providers
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <param name="providerName">Optional provider name for multi-provider scenarios</param>
    /// <param name="configureOptions">Optional delegate to configure GitHub options</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddGitHubContentProvider(
        this IServiceCollection services,
        IConfiguration configuration,
        string? providerName = null,
        Action<GitHubOptions>? configureOptions = null)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (configuration is null) throw new ArgumentNullException(nameof(configuration));

        // Register the factory if not already registered
        services.TryAddSingleton<IGitHubApiClientFactory, GitHubApiClientFactory>();

        // Register HTTP client factory for creating named clients
        services.TryAddHttpClient<IGitHubTokenProvider, GitHubTokenProvider>();
        services.TryAddHttpClient<IAuthenticationService, AuthenticationService>();
        services.TryAddHttpClient<IGitHubAdminService, GitHubAdminService>();

        // If provider name is specified, register as a named provider
        if (!string.IsNullOrWhiteSpace(providerName))
        {
            // Configure options for this specific provider
            services.Configure<GitHubOptions>($"GitHub_{providerName}", options => {
                // Try to load from new structure first
                var providerSection = configuration.GetSection($"Osirion:Cms:Web:GitHub:{providerName}");
                if (!providerSection.Exists())
                {
                    providerSection = configuration.GetSection($"Osirion:Cms:Admin:GitHub:{providerName}");
                }

                if (providerSection.Exists())
                {
                    providerSection.Bind(options);
                }

                // Apply custom configuration
                configureOptions?.Invoke(options);
            });
        }
        else
        {
            // Legacy registration - configure default options
            if (configureOptions is not null)
            {
                services.Configure<GitHubOptions>(options => {
                    // First apply configuration from settings
                    configuration.GetSection(GitHubOptions.Section).Bind(options);
                    // Then apply custom configuration
                    configureOptions(options);
                });
            }
            else
            {
                services.Configure<GitHubOptions>(configuration.GetSection(GitHubOptions.Section));
            }
        }

        // Register GitHub repositories (they will use the factory)
        services.TryAddSingleton<GitHubContentRepository>();
        services.TryAddSingleton<GitHubDirectoryRepository>();
        services.TryAddSingleton<IDirectoryRepository, GitHubDirectoryRepository>();

        // Register provider
        services.TryAddSingleton<GitHubContentProvider>();

        // Register as IContentProvider
        services.TryAddSingleton<IContentProvider>(
            sp => sp.GetRequiredService<GitHubContentProvider>());

        // Register default setter
        services.TryAddSingleton<IDefaultProviderSetter>(sp => {
            var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<GitHubOptions>>().Value;
            var providerId = options.ProviderId ?? $"github-{options.Owner}-{options.Repository}";
            return new DefaultProviderSetter(providerId, options.IsDefault);
        });

        services.AddBlazoredLocalStorage();

        return services;
    }

    /// <summary>
    /// Adds FileSystem content provider
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <param name="configureOptions">Optional delegate to configure FileSystem options</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddFileSystemContentProvider(
    this IServiceCollection services,
    IConfiguration configuration,
    Action<FileSystemOptions>? configureOptions = null)
    {
        // Configure options
        if (configureOptions is not null)
        {
            services.Configure<FileSystemOptions>(options => {
                // First apply configuration from settings
                configuration.GetSection(FileSystemOptions.Section).Bind(options);
                // Then apply custom configuration
                configureOptions(options);
            });
        }
        else
        {
            services.Configure<FileSystemOptions>(configuration.GetSection(FileSystemOptions.Section));
        }

        // Register FileSystem repositories
        services.TryAddScoped<FileSystemContentRepository>();
        services.TryAddScoped<FileSystemDirectoryRepository>();

        // Register provider
        services.TryAddScoped<FileSystemContentProvider>();

        // Register as IContentProvider
        services.AddScoped<IContentProvider>(sp => sp.GetRequiredService<FileSystemContentProvider>());

        // Register default setter
        services.AddSingleton<IDefaultProviderSetter>(sp => {
            var options = sp.GetRequiredService<IOptions<FileSystemOptions>>().Value;
            var providerId = options.ProviderId ?? $"filesystem-{options.BasePath.GetHashCode():x}";
            return new DefaultProviderSetter(providerId, options.IsDefault);
        });

        return services;
    }

    /// <summary>
    /// Adds a custom content provider
    /// </summary>
    /// <typeparam name="TProvider">The type of the provider</typeparam>
    /// <param name="services">The service collection</param>
    /// <param name="configure">Optional delegate to configure the provider</param>
    /// <param name="isDefault">Whether this provider should be the default</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddCustomContentProvider<TProvider>(
        this IServiceCollection services,
        Action<TProvider>? configure = null,
        bool isDefault = false)
        where TProvider : class, IContentProvider
    {
        if (services is null) throw new ArgumentNullException(nameof(services));

        // Register the provider
        services.TryAddScoped<TProvider>();

        // Register as IContentProvider
        services.AddScoped<IContentProvider>(sp => {
            var provider = sp.GetRequiredService<TProvider>();

            // Apply configuration if provided
            configure?.Invoke(provider);

            return provider;
        });

        // If default, register the default setter
        if (isDefault)
        {
            services.AddSingleton<IDefaultProviderSetter>(sp => {
                var provider = sp.GetRequiredService<TProvider>();
                return new DefaultProviderSetter(provider.ProviderId, true);
            });
        }

        return services;
    }

    /// <summary>
    /// Adds Osirion CMS services with all configured providers from configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirionCmsWithProviders(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (configuration is null) throw new ArgumentNullException(nameof(configuration));

        // Add core CMS services
        services.AddCms(configuration);

        // Explicitly register providers based on configuration
        AutoRegisterProviders(services, configuration);

        return services;
    }

    /// <summary>
    /// Helper method to automatically register providers based on configuration
    /// </summary>
    private static void AutoRegisterProviders(IServiceCollection services, IConfiguration configuration)
    {
        var cmsSection = configuration.GetSection("Osirion:Cms");

        // Check for GitHub provider
        var githubSection = cmsSection.GetSection("GitHub");
        if (githubSection.Exists())
        {
            services.AddGitHubContentProvider(configuration);
        }

        // Check for FileSystem provider
        var fileSystemSection = cmsSection.GetSection("FileSystem");
        if (fileSystemSection.Exists())
        {
            services.AddFileSystemContentProvider(configuration);
        }
    }
}