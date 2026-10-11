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
        Should.Throw<ArgumentNullException>(() => new DefaultProviderSetter(null!, true));
    }

    [Fact]
    public void SetDefault_WhenIsDefault_SetsDefaultProvider()
    {
        // Arrange
        var providerId = "test-provider";
        var setter = new DefaultProviderSetter(providerId, true);

        var registry = Substitute.For<IContentProviderRegistry>();
        var registryResolutions = 0;
        using var serviceProvider = new ServiceCollection()
            .AddSingleton(_ =>
            {
                registryResolutions++;
                return registry;
            })
            .BuildServiceProvider();

        // Act
        setter.SetDefault(serviceProvider);

        // Assert
        registryResolutions.ShouldBe(1);
        registry.Received(1).SetDefaultProvider(providerId);
    }

    [Fact]
    public void SetDefault_WhenNotDefault_DoesNotSetDefaultProvider()
    {
        // Arrange
        var providerId = "test-provider";
        var setter = new DefaultProviderSetter(providerId, false);

        var registry = Substitute.For<IContentProviderRegistry>();
        var registryResolutions = 0;
        using var serviceProvider = new ServiceCollection()
            .AddSingleton(_ =>
            {
                registryResolutions++;
                return registry;
            })
            .BuildServiceProvider();

        // Act
        setter.SetDefault(serviceProvider);

        // Assert
        registryResolutions.ShouldBe(0);
        registry.DidNotReceive().SetDefaultProvider(Arg.Any<string>());
    }
}
