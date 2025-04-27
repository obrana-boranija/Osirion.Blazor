using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Infrastructure.Providers.FileSystem;
using Osirion.Blazor.Cms.Infrastructure.Providers.GitHub;

namespace Osirion.Blazor.Cms.Infrastructure.Providers.Strategy
{
    /// <summary>
    /// Allows runtime selection of a provider (Strategy pattern).
    /// Avoids a “ProviderManager” Middle‐Man smell (SoC) :contentReference[oaicite:5]{index=5}.
    /// </summary>
    public interface IContentProviderStrategy
    {
        IReadContentProvider GetProvider(string key);
    }

    public class ContentProviderStrategy : IContentProviderStrategy
    {
        private readonly IServiceProvider _services;

        public ContentProviderStrategy(IServiceProvider services)
            => _services = services;

        public IReadContentProvider GetProvider(string key) =>
            key switch
            {
                "FileSystem" => _services.GetRequiredService<FileSystemContentProvider>(),
                "GitHub" => _services.GetRequiredService<GitHubContentProvider>(),
                _ => throw new NotSupportedException(key)
            };
    }
}
