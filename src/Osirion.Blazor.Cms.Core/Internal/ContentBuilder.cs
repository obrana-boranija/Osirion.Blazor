using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Options;
using Osirion.Blazor.Cms.Providers;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;

namespace Osirion.Blazor.Cms.Internal;

/// <summary>
/// Implementation of the content builder
/// </summary>
internal class ContentBuilder : IContentBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ContentBuilder"/> class.
    /// </summary>
    public ContentBuilder(IServiceCollection services)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
    }

    /// <inheritdoc/>
    public IServiceCollection Services { get; }

    /// <inheritdoc/>
    public IContentBuilder AddGitHub(Action<GitHubContentOptions>? configure = null)
    {
        var options = new GitHubContentOptions();
        configure?.Invoke(options);

        Services.Configure<GitHubContentOptions>(opt =>
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
        });

        // Configure HttpClient properly during registration
        Services.AddHttpClient<GitHubContentProvider>()
            .ConfigureHttpClient((serviceProvider, client) =>
            {
                client.BaseAddress = new Uri("https://api.github.com/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("OsirionBlazor", "2.0"));

                var gitHubOptions = serviceProvider.GetRequiredService<IOptions<GitHubContentOptions>>().Value;
                if (!string.IsNullOrEmpty(gitHubOptions.ApiToken))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", gitHubOptions.ApiToken);
                }
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                if (OperatingSystem.IsBrowser())
                {
                    return new HttpClientHandler(); // Use a compatible handler for browser environments
                }
                return new SocketsHttpHandler
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(15)
                };
            });

        // Register as singleton
        Services.AddSingleton<IContentProvider>(sp =>
        sp.GetRequiredService<GitHubContentProvider>());

        return this;
        
    }

    /// <inheritdoc/>
    public IContentBuilder AddFileSystem(Action<FileSystemContentOptions>? configure = null)
    {
        var options = new FileSystemContentOptions();
        configure?.Invoke(options);

        Services.Configure<FileSystemContentOptions>(opt =>
        {
            opt.BasePath = options.BasePath;
            opt.WatchForChanges = options.WatchForChanges;
            opt.ProviderId = options.ProviderId;
            opt.EnableCaching = options.EnableCaching;
            opt.CacheDurationMinutes = options.CacheDurationMinutes;
            opt.SupportedExtensions = options.SupportedExtensions;
        });

        Services.AddSingleton<IContentProvider, FileSystemContentProvider>();

        return this;
    }

    /// <inheritdoc/>
    public IContentBuilder AddProvider<TProvider>(Action<TProvider>? configure = null)
        where TProvider : class, IContentProvider
    {
        if (configure != null)
        {
            Services.AddSingleton(sp => {
                var provider = ActivatorUtilities.CreateInstance<TProvider>(sp);
                configure(provider);
                return provider;
            });
        }
        else
        {
            Services.AddSingleton<TProvider>();
        }

        Services.AddSingleton<IContentProvider>(sp =>
            sp.GetRequiredService<TProvider>());

        return this;
    }
}