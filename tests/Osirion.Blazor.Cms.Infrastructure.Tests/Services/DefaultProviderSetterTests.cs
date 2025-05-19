using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Infrastructure.Services;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Services;

public class DefaultProviderSetterTests
{
    [Fact]
    public void Constructor_InitializesProperties()
    {
        // Arrange & Act
        var providerId = "test-provider";
        var isDefault = true;
        var setter = new DefaultProviderSetter(providerId, isDefault);

        // Assert - using reflection to check private fields
        var idField = typeof(DefaultProviderSetter).GetField("_providerId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var defaultField = typeof(DefaultProviderSetter).GetField("_isDefault", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        idField.ShouldNotBeNull();
        defaultField.ShouldNotBeNull();

        idField.GetValue(setter).ShouldBe(providerId);
        defaultField.GetValue(setter).ShouldBe(isDefault);
    }

    [Fact]
    public void Constructor_WithNullProviderId_ThrowsArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new DefaultProviderSetter(null, true));
    }

    [Fact]
    public void SetDefault_WhenIsDefault_SetsDefaultProvider()
    {
        // Arrange
        var providerId = "test-provider";
        var setter = new DefaultProviderSetter(providerId, true);

        var registry = Substitute.For<IContentProviderRegistry>();
        var serviceProvider = Substitute.For<IServiceProvider>();
        serviceProvider.GetRequiredService<IContentProviderRegistry>().Returns(registry);

        // Act
        setter.SetDefault(serviceProvider);

        // Assert
        serviceProvider.Received(1).GetRequiredService<IContentProviderRegistry>();
        registry.Received(1).SetDefaultProvider(providerId);
    }

    [Fact]
    public void SetDefault_WhenNotDefault_DoesNotSetDefaultProvider()
    {
        // Arrange
        var providerId = "test-provider";
        var setter = new DefaultProviderSetter(providerId, false);

        var registry = Substitute.For<IContentProviderRegistry>();
        var serviceProvider = Substitute.For<IServiceProvider>();
        serviceProvider.GetRequiredService<IContentProviderRegistry>().Returns(registry);

        // Act
        setter.SetDefault(serviceProvider);

        // Assert
        serviceProvider.DidNotReceive().GetRequiredService<IContentProviderRegistry>();
        registry.DidNotReceive().SetDefaultProvider(Arg.Any<string>());
    }
}