using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Configuration;
using Osirion.Blazor.Cms.Admin.DependencyInjection;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Admin.Builders;

/// <summary>
/// Builder implementation for configuring CMS admin features
/// </summary>
public class AdminBuilder : IAdminBuilder
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AdminBuilder> _logger;

    public IServiceCollection Services { get; }

    public AdminBuilder(
        IServiceCollection services,
        IConfiguration configuration,
        ILogger<AdminBuilder> logger)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IAdminBuilder UseGitHubProvider(Action<GitHubAdminOptions> configure = null)
    {
        _logger.LogInformation("Configuring GitHub provider for CMS admin");

        // Register GitHub services from Infrastructure
        Services.AddGitHubAdminServices(_configuration);

        if (configure != null)
        {
            Services.Configure<GitHubAdminOptions>(options =>
            {
                options.DefaultContentProvider = "GitHub";
                configure(options);
            });
        }

        return this;
    }

    public IAdminBuilder UseFileSystemProvider(Action<FileSystemAdminOptions> configure = null)
    {
        _logger.LogInformation("Configuring FileSystem provider for CMS admin");

        // Register FileSystem services from Infrastructure
        Services.AddFileSystemAdminServices(_configuration);

        if (configure != null)
        {
            Services.Configure<FileSystemAdminOptions>(options =>
            {
                options.DefaultContentProvider = "FileSystem";
                configure(options);
            });
        }

        return this;
    }

    public IAdminBuilder ConfigureAuthentication(Action<AuthenticationOptions> configure)
    {
        if (configure == null)
            throw new ArgumentNullException(nameof(configure));

        _logger.LogInformation("Configuring authentication for CMS admin");

        Services.Configure<AuthenticationOptions>(configure);

        return this;
    }

    public IAdminBuilder ConfigureTheme(Action<ThemeOptions> configure)
    {
        if (configure == null)
            throw new ArgumentNullException(nameof(configure));

        _logger.LogInformation("Configuring theme for CMS admin");

        Services.Configure<ThemeOptions>(configure);

        return this;
    }

    public IAdminBuilder AddProvider<TProvider>() where TProvider : class, IContentProvider
    {
        _logger.LogInformation("Adding custom provider {ProviderType} to CMS admin", typeof(TProvider).Name);

        Services.AddScoped<IContentProvider, TProvider>();

        return this;
    }
}