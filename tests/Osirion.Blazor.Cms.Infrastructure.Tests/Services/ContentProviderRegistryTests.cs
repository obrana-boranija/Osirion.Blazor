using Microsoft.Extensions.Logging;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Infrastructure.Services;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Services
{
    public class ContentProviderRegistryTests
    {
        private readonly ILogger<ContentProviderRegistry> _logger;
        private readonly ContentProviderRegistry _registry;
        private readonly IContentProvider _provider1;
        private readonly IContentProvider _provider2;

        public ContentProviderRegistryTests()
        {
            _logger = Substitute.For<ILogger<ContentProviderRegistry>>();

            // Create mock providers
            _provider1 = Substitute.For<IContentProvider>();
            _provider1.ProviderId.Returns("provider1");
            _provider1.DisplayName.Returns("Provider 1");

            _provider2 = Substitute.For<IContentProvider>();
            _provider2.ProviderId.Returns("provider2");
            _provider2.DisplayName.Returns("Provider 2");

            // Create registry with mock providers
            _registry = new ContentProviderRegistry(
                new List<IContentProvider> { _provider1, _provider2 },
                _logger);
        }

        [Fact]
        public void GetAllProviders_ReturnsAllRegisteredProviders()
        {
            // Act
            var providers = _registry.GetAllProviders();

            // Assert
            providers.ShouldNotBeNull();
            providers.Count().ShouldBe(2);
            providers.ShouldContain(_provider1);
            providers.ShouldContain(_provider2);
        }

        [Fact]
        public void GetDefaultProvider_ReturnsFirstProviderByDefault()
        {
            // Act
            var defaultProvider = _registry.GetDefaultProvider();

            // Assert
            defaultProvider.ShouldBe(_provider1);
        }

        [Fact]
        public void SetDefaultProvider_ChangesDefaultProvider()
        {
            // Act
            _registry.SetDefaultProvider("provider2");
            var defaultProvider = _registry.GetDefaultProvider();

            // Assert
            defaultProvider.ShouldBe(_provider2);
        }

        [Fact]
        public void GetProvider_WithValidId_ReturnsCorrectProvider()
        {
            // Act
            var provider = _registry.GetProvider("provider2");

            // Assert
            provider.ShouldBe(_provider2);
        }

        [Fact]
        public void GetProvider_WithInvalidId_ReturnsNull()
        {
            // Act
            var provider = _registry.GetProvider("nonexistent");

            // Assert
            provider.ShouldBeNull();
        }

        [Fact]
        public void SetDefaultProvider_WithInvalidId_ThrowsException()
        {
            // Act & Assert
            Should.Throw<Osirion.Blazor.Cms.Domain.Exceptions.ContentProviderException>(() =>
                _registry.SetDefaultProvider("nonexistent"));
        }
    }
}