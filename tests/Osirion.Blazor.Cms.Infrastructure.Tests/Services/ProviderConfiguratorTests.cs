using NSubstitute;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Infrastructure.Services;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Services
{
    public class ProviderConfiguratorTests
    {
        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenConfigureIsNull()
        {
            // Act & Assert
            Should.Throw<ArgumentNullException>(() =>
                new ProviderConfigurator<IContentProvider>(null!));
        }

        [Fact]
        public void Configure_CallsConfigureAction()
        {
            // Arrange
            var provider = Substitute.For<IContentProvider>();
            bool configureCalled = false;

            var configurator = new ProviderConfigurator<IContentProvider>(p => {
                p.ShouldBeSameAs(provider);
                configureCalled = true;
            });

            // Act
            configurator.Configure(provider);

            // Assert
            configureCalled.ShouldBeTrue();
        }

        [Fact]
        public void Configure_PassesProviderToConfigureAction()
        {
            // Arrange
            var provider = Substitute.For<IContentProvider>();
            provider.ProviderId.Returns("test-provider");
            IContentProvider capturedProvider = null;

            var configurator = new ProviderConfigurator<IContentProvider>(p => {
                capturedProvider = p;
            });

            // Act
            configurator.Configure(provider);

            // Assert
            capturedProvider.ShouldBe(provider);
            capturedProvider?.ProviderId.ShouldBe("test-provider");
        }

        // Test with a specific provider implementation
        [Fact]
        public void Configure_WorksWithSpecificProviderType()
        {
            // Arrange
            var mockProvider = Substitute.For<TestContentProvider>();
            var configureCalled = false;

            var configurator = new ProviderConfigurator<TestContentProvider>(p => {
                p.ShouldBeSameAs(mockProvider);
                configureCalled = true;
            });

            // Act
            configurator.Configure(mockProvider);

            // Assert
            configureCalled.ShouldBeTrue();
        }

        // Helper test implementation
        public interface TestContentProvider : IContentProvider
        {
            void DoSomething();
        }
    }
}