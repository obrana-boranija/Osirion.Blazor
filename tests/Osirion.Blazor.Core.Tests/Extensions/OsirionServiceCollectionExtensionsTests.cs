using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Extensions;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Extensions;

public class OsirionServiceCollectionExtensionsTests
{
    [Fact]
    public void AddOsirion_WithNullServices_ShouldThrowArgumentNullException()
    {
        // Arrange
        IServiceCollection? services = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => services!.AddOsirion());
    }

    [Fact]
    public void AddOsirion_WithValidServices_ShouldReturnServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddOsirion();

        // Assert
        result.ShouldBe(services);
    }

    [Fact]
    public void AddOsirion_WithConfiguration_ShouldInvokeConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();
        var configurationCalled = false;

        // Act
        services.AddOsirion(builder =>
        {
            configurationCalled = true;
            builder.Services.ShouldBe(services);
        });

        // Assert
        configurationCalled.ShouldBeTrue();
    }
}