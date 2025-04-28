using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Domain.Options;

namespace Osirion.Blazor.Cms.Infrastructure.DependencyInjection;

/// <summary>
/// Builder for configuring Osirion CMS services
/// </summary>
public class OsirionCmsBuilder
{
    public IServiceCollection Services { get; }
    public IConfiguration Configuration { get; }

    public OsirionCmsBuilder(IServiceCollection services, IConfiguration configuration)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Adds GitHub provider
    /// </summary>
    public OsirionCmsBuilder AddGitHub(Action<GitHubOptions>? configure = null)
    {
        Services.AddGitHubContentProvider(Configuration, configure);
        return this;
    }

    /// <summary>
    /// Adds FileSystem provider
    /// </summary>
    public OsirionCmsBuilder AddFileSystem(Action<FileSystemOptions>? configure = null)
    {
        Services.AddFileSystemContentProvider(Configuration, configure);
        return this;
    }

    /// <summary>
    /// Adds custom content provider
    /// </summary>
    public OsirionCmsBuilder AddProvider<TProvider>(Action<TProvider>? configure = null)
        where TProvider : class, Domain.Services.IContentProvider
    {
        // Register the provider
        Services.AddScoped<TProvider>();

        // Register as the interface
        Services.AddScoped<Domain.Services.IContentProvider>(sp => {
            var provider = sp.GetRequiredService<TProvider>();

            // Apply configuration if provided
            if (configure != null)
            {
                configure(provider);
            }

            return provider;
        });

        return this;
    }

    /// <summary>
    /// Sets the default provider
    /// </summary>
    public OsirionCmsBuilder SetDefaultProvider(string providerId)
    {
        // Register a startup hook to set the default provider
        Services.AddSingleton<Domain.Interfaces.IDefaultProviderSetter>(
            new Services.DefaultProviderSetter(providerId, true));

        return this;
    }
}