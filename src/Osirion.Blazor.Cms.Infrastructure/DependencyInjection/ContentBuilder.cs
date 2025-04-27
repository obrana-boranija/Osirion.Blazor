using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Infrastructure.Caching;
using Osirion.Blazor.Cms.Infrastructure.FileSystem;
using Osirion.Blazor.Cms.Infrastructure.GitHub;

namespace Osirion.Blazor.Cms.Infrastructure.DependencyInjection;

/// <summary>
/// Builder implementation for configuring content providers
/// </summary>
public class ContentBuilder : IContentBuilder
{
    public IServiceCollection Services { get; }
    private readonly IContentProviderFactory _providerFactory;
    private readonly IConfiguration _configuration;
    private readonly CacheDecoratorFactory _cacheFactory;

    /// <summary>
    /// Creates a new ContentBuilder instance
    /// </summary>
    /// <param name="services">The service collection being configured</param>
    /// <param name="configuration">The configuration</param>
    /// <param name="providerFactory">The content provider factory</param>
    /// <param name="cacheFactory">The cache decorator factory</param>
    public ContentBuilder(
        IServiceCollection services,
        IConfiguration configuration,
        IContentProviderFactory providerFactory,
        CacheDecoratorFactory cacheFactory)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _providerFactory = providerFactory ?? throw new ArgumentNullException(nameof(providerFactory));
        _cacheFactory = cacheFactory ?? throw new ArgumentNullException(nameof(cacheFactory));
    }

    /// <inheritdoc/>
    public IContentBuilder AddGitHub(Action<GitHubOptions>? configure = null)
    {
        // Configure GitHub options
        Services.Configure<GitHubOptions>(options =>
        {
            // Set default values from configuration
            _configuration.GetSection("Osirion:Cms:GitHub").Bind(options);

            // Apply custom configuration if provided
            configure?.Invoke(options);
        });

        // Register GitHub API client
        Services.AddHttpClient<IGitHubApiClient, GitHubApiClient>();

        // Register repositories
        Services.AddScoped<GitHubContentRepository>();
        Services.AddScoped<GitHubDirectoryRepository>();

        // Register with provider factory
        //_providerFactory.RegisterProvider<GitHubContentProvider>(sp =>
        //{
        //    var options = sp.GetRequiredService<IOptions<GitHubOptions>>();
        //    var memoryCache = sp.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>();
        //    var contentRepo = sp.GetRequiredService<GitHubContentRepository>();
        //    var directoryRepo = sp.GetRequiredService<GitHubDirectoryRepository>();
        //    var logger = sp.GetRequiredService<ILogger<GitHubContentProvider>>();
        //    var markdownProcessor = sp.GetRequiredService<IMarkdownProcessor>();

        //    // Apply caching if enabled
        //    if (options.Value.EnableCaching)
        //    {
        //        contentRepo = (GitHubContentRepository)_cacheFactory.CreateContentCacheDecorator(contentRepo,
        //            options.Value.ProviderId ?? $"github-{options.Value.Owner}-{options.Value.Repository}");
        //        directoryRepo = (GitHubDirectoryRepository)_cacheFactory.CreateDirectoryCacheDecorator(directoryRepo,
        //            options.Value.ProviderId ?? $"github-{options.Value.Owner}-{options.Value.Repository}");
        //    }

        //    return new GitHubContentProvider(_configuration);
        //});

        return this;
    }

    /// <inheritdoc/>
    public IContentBuilder AddFileSystem(Action<FileSystemOptions>? configure = null)
    {
        // Configure FileSystem options
        Services.Configure<FileSystemOptions>(options =>
        {
            // Set default values from configuration
            _configuration.GetSection("Osirion:Cms:FileSystem").Bind(options);

            // Apply custom configuration if provided
            configure?.Invoke(options);
        });

        // Register repositories
        Services.AddScoped<FileSystemContentRepository>();
        Services.AddScoped<FileSystemDirectoryRepository>();

        // Register with provider factory
        //_providerFactory.RegisterProvider<FileSystemContentProvider>(sp =>
        //{
        //    var options = sp.GetRequiredService<IOptions<FileSystemOptions>>();
        //    var memoryCache = sp.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>();
        //    var contentRepo = sp.GetRequiredService<FileSystemContentRepository>();
        //    var directoryRepo = sp.GetRequiredService<FileSystemDirectoryRepository>();
        //    var logger = sp.GetRequiredService<ILogger<FileSystemContentProvider>>();
        //    var markdownProcessor = sp.GetRequiredService<IMarkdownProcessor>();

        //    // Apply caching if enabled
        //    if (options.Value.EnableCaching)
        //    {
        //        contentRepo = (FileSystemContentRepository)_cacheFactory.CreateContentCacheDecorator(contentRepo,
        //            options.Value.ProviderId ?? $"filesystem-{options.Value.BasePath.GetHashCode():x}");
        //        directoryRepo = (FileSystemDirectoryRepository)_cacheFactory.CreateDirectoryCacheDecorator(directoryRepo,
        //            options.Value.ProviderId ?? $"filesystem-{options.Value.BasePath.GetHashCode():x}");
        //    }

        //    return new FileSystemContentProvider(_configuration);
        //});

        return this;
    }

    /// <inheritdoc/>
    public IContentBuilder AddProvider<TProvider>(Action<TProvider>? configure = null)
        where TProvider : class, IContentProvider
    {
        // Register the provider
        Services.AddScoped<TProvider>();

        // Apply configuration if provided
        if (configure != null)
        {
            Services.AddTransient(sp =>
            {
                var provider = sp.GetRequiredService<TProvider>();
                configure(provider);
                return provider;
            });
        }

        // Register the provider with the factory
        _providerFactory.RegisterProvider<TProvider>(sp => sp.GetRequiredService<TProvider>());

        return this;
    }

    /// <inheritdoc/>
    public IContentBuilder SetDefaultProvider<TProvider>()
        where TProvider : class, IContentProvider
    {
        // Get a temporary service provider to resolve an instance
        using var scope = Services.BuildServiceProvider().CreateScope();
        var provider = scope.ServiceProvider.GetRequiredService<TProvider>();

        // Set the provider's ID as default
        _providerFactory.SetDefaultProvider(provider.ProviderId);

        return this;
    }

    /// <inheritdoc/>
    public IContentBuilder SetDefaultProvider(string providerId)
    {
        _providerFactory.SetDefaultProvider(providerId);
        return this;
    }
}