using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Interfaces;
using Osirion.Blazor.Cms.Providers;

namespace Osirion.Blazor.Cms.Internal
{
    /// <summary>
    /// Factory for creating content providers
    /// </summary>
    internal class ContentProviderFactory : IContentProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, Func<IServiceProvider, IContentProvider>> _factories = new();

        public ContentProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <inheritdoc/>
        public IContentProvider CreateProvider(string providerId)
        {
            if (_factories.TryGetValue(providerId, out var factory))
            {
                return factory(_serviceProvider);
            }

            throw new InvalidOperationException($"Content provider '{providerId}' is not registered.");
        }

        /// <inheritdoc/>
        public IEnumerable<string> GetAvailableProviderTypes() => _factories.Keys;

        /// <inheritdoc/>
        public void RegisterProvider<T>(Func<IServiceProvider, T> factory) where T : class, IContentProvider
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            // Create a temporary instance to get the provider ID
            var tempProvider = factory(_serviceProvider);
            _factories[tempProvider.ProviderId] = sp => factory(sp);
        }
    }

    
}