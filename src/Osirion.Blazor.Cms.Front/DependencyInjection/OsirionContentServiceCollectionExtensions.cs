using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Infrastructure.Builders;
using Osirion.Blazor.Cms.Infrastructure.Caching;
using Osirion.Blazor.Cms.Infrastructure.Services;

namespace Osirion.Blazor.Cms.Front.DependencyInjection;

public static class OsirionContentServiceCollectionExtensions
{
    public static IServiceCollection AddOsirionContent(this IServiceCollection services, Action<IContentBuilder> configure)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        //services.AddOsirionContent(configure);
        services.AddSingleton<IContentProviderFactory, ContentProviderFactory>();
        services.AddSingleton<CacheDecoratorFactory>();
        services.AddSingleton<IContentBuilder, ContentBuilder>();

        return services;
    }

    public static IServiceCollection AddOsirionContent(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        // Example logic: Configure services based on the configuration
        var section = configuration.GetSection(GitHubOptions.Section);
        if (section.Exists())
        {
            services.Configure<GitHubOptions>(section);
        }

        return services;
    }
}
