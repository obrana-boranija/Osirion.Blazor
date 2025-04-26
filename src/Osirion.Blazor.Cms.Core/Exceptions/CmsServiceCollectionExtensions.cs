using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Core.Interfaces;
using Osirion.Blazor.Cms.Core.Options;
using Osirion.Blazor.Cms.Interfaces;
using Osirion.Blazor.Cms.Options;
using Osirion.Blazor.Cms.Providers;
using Osirion.Blazor.Cms.Providers.GitHub;
using Osirion.Blazor.Cms.Services;

namespace Osirion.Blazor.Cms.Extensions;

/// <summary>
/// Extension methods for configuring CMS services in dependency injection
/// </summary>
public static class CmsServiceCollectionExtensions
{
    /// <summary>
    /// Adds core CMS services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configure">Action to configure content providers</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirionContent(
        this IServiceCollection services,
        Action<IContentBuilder> configure)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        // Register common services
        services.AddMemoryCache();
        services.TryAddSingleton<IContentCacheService, ContentCacheService>();
        services.TryAddSingleton<IContentParser, ContentParser>();
        services.TryAddScoped<IContentProviderManager, ContentProviderManager>();
        services.TryAddSingleton<IContentProviderFactory, ContentProviderFactory>();

        // Create builder and apply configuration
        var builder = new ContentBuilder(services);
        configure(builder);

        return services;
    }

    /// <summary>
    /// Adds core CMS services from configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOsirionContent(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        // Register caching options
        services.Configure<ContentCacheOptions>(configuration.GetSection("Osirion:Cms:Caching"));

        return services.AddOsirionContent(builder =>
        {
            // Configure GitHub provider if in config
            var githubSection = configuration.GetSection("Osirion:Cms:GitHub");
            if (githubSection.Exists())
            {
                builder.AddGitHub(github =>
                {
                    githubSection.Bind(github);
                });
            }

            // Configure file system provider if in config
            var fileSystemSection = configuration.GetSection("Osirion:Cms:FileSystem");
            if (fileSystemSection.Exists())
            {
                builder.AddFileSystem(fileSystem =>
                {
                    fileSystemSection.Bind(fileSystem);
                });
            }
        });
    }
}

/// <summary>
/// Implementation of the content provider factory
/// </summary>
internal class ContentProviderFactory : IContentProviderFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, Func<IServiceProvider, IContentProvider>> _factories = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentProviderFactory"/> class.
    /// </summary>
    public ContentProviderFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <inheritdoc/>
    public IContentProvider CreateProvider(string providerId)
    {
        if (string.IsNullOrEmpty(providerId))
            throw new ArgumentException("Provider ID cannot be null or empty", nameof(providerId));

        if (_factories.TryGetValue(providerId, out var factory))
        {
            return factory(_serviceProvider);
        }

        throw new KeyNotFoundException($"No provider registered with ID '{providerId}'");
    }

    /// <inheritdoc/>
    public IEnumerable<string> GetAvailableProviderTypes()
    {
        return _factories.Keys;
    }

    /// <inheritdoc/>
    public void RegisterProvider<T>(Func<IServiceProvider, T> factory) where T : class, IContentProvider
    {
        if (factory == null) throw new ArgumentNullException(nameof(factory));

        // Create a temporary instance to get the provider ID
        var tempProvider = factory(_serviceProvider);
        var providerId = tempProvider.ProviderId;

        if (string.IsNullOrEmpty(providerId))
            throw new ArgumentException("Provider ID cannot be null or empty");

        _factories[providerId] = factory;
    }
}

/// <summary>
/// Implementation of the content builder
/// </summary>
internal class ContentBuilder : IContentBuilder
{
    private readonly IServiceCollection _services;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentBuilder"/> class.
    /// </summary>
    public ContentBuilder(IServiceCollection services)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
    }

    /// <inheritdoc/>
    public IServiceCollection Services => _services;

    /// <inheritdoc/>
    public IContentBuilder AddGitHub(Action<GitHubContentOptions>? configure = null)
    {
        var options = new GitHubContentOptions();
        configure?.Invoke(options);

        // Register options
        _services.Configure<GitHubContentOptions>(opt =>
        {
            opt.Owner = options.Owner;
            opt.Repository = options.Repository;
            opt.ContentPath = options.ContentPath;
            opt.Branch = options.Branch;
            opt.ApiToken = options.ApiToken;
            opt.ProviderId = options.ProviderId;
            opt.EnableCaching = options.EnableCaching;
            opt.CacheDurationMinutes = options.CacheDurationMinutes;
            opt.SupportedExtensions = options.SupportedExtensions;
            opt.EnableLocalization = options.EnableLocalization;
            opt.DefaultLocale = options.DefaultLocale;
        });

        // Register HTTP client
        _services.AddHttpClient<IGitHubApiClient, GitHubApiClient>();

        // Register provider
        _services.AddSingleton<GitHubContentProvider>();
        _services.AddSingleton<IContentProvider>(sp =>
            sp.GetRequiredService<GitHubContentProvider>());

        // Register with provider factory
        _services.AddSingleton<IContentProviderRegistration>(sp =>
        {
            var factory = sp.GetRequiredService<IContentProviderFactory>();
            var providerOptions = sp.GetRequiredService<IOptions<GitHubContentOptions>>();
            var providerId = providerOptions.Value.ProviderId ?? $"github-{providerOptions.Value.Owner}-{providerOptions.Value.Repository}";

            factory.RegisterProvider<IContentProvider>(s =>
                s.GetRequiredService<GitHubContentProvider>());

            return new ContentProviderRegistration(providerId);
        });

        return this;
    }

    /// <inheritdoc/>
    public IContentBuilder AddFileSystem(Action<FileSystemContentOptions>? configure = null)
    {
        var options = new FileSystemContentOptions();
        configure?.Invoke(options);

        // Register options
        _services.Configure<FileSystemContentOptions>(opt =>
        {
            opt.BasePath = options.BasePath;
            opt.WatchForChanges = options.WatchForChanges;
            opt.ProviderId = options.ProviderId;
            opt.EnableCaching = options.EnableCaching;
            opt.CacheDurationMinutes = options.CacheDurationMinutes;
            opt.SupportedExtensions = options.SupportedExtensions;
        });

        // Register provider
        _services.AddSingleton<FileSystemContentProvider>();
        _services.AddSingleton<IContentProvider>(sp =>
            sp.GetRequiredService<FileSystemContentProvider>());

        // Register with provider factory
        _services.AddSingleton<IContentProviderRegistration>(sp =>
        {
            var factory = sp.GetRequiredService<IContentProviderFactory>();
            var providerOptions = sp.GetRequiredService<IOptions<FileSystemContentOptions>>();
            var providerId = providerOptions.Value.ProviderId ??
                $"filesystem-{providerOptions.Value.BasePath.GetHashCode():x}";

            factory.RegisterProvider<IContentProvider>(s =>
                s.GetRequiredService<FileSystemContentProvider>());

            return new ContentProviderRegistration(providerId);
        });

        return this;
    }

    /// <inheritdoc/>
    public IContentBuilder AddProvider<TProvider>(Action<TProvider>? configure = null)
        where TProvider : class, IContentProvider
    {
        // Register provider
        if (configure != null)
        {
            _services.AddSingleton(sp =>
            {
                var provider = ActivatorUtilities.CreateInstance<TProvider>(sp);
                configure(provider);
                return provider;
            });
        }
        else
        {
            _services.AddSingleton<TProvider>();
        }

        _services.AddSingleton<IContentProvider>(sp =>
            sp.GetRequiredService<TProvider>());

        // Register with provider factory
        _services.AddSingleton<IContentProviderRegistration>(sp =>
        {
            var factory = sp.GetRequiredService<IContentProviderFactory>();
            var provider = sp.GetRequiredService<TProvider>();

            factory.RegisterProvider<IContentProvider>(s =>
                s.GetRequiredService<TProvider>());

            return new ContentProviderRegistration(provider.ProviderId);
        });

        return this;
    }
}

/// <summary>
/// Helper class for provider registration
/// </summary>
internal class ContentProviderRegistration : IContentProviderRegistration
{
    /// <summary>
    /// Gets the provider ID
    /// </summary>
    public string ProviderId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentProviderRegistration"/> class.
    /// </summary>
    public ContentProviderRegistration(string providerId)
    {
        ProviderId = providerId ?? throw new ArgumentNullException(nameof(providerId));
    }
}

/// <summary>
/// Interface for provider registration
/// </summary>
internal interface IContentProviderRegistration
{
    /// <summary>
    /// Gets the provider ID
    /// </summary>
    string ProviderId { get; }
}