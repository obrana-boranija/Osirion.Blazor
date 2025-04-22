using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Osirion.Blazor.Core.Extensions;

namespace Osirion.Blazor.Core.Tests.Extensions;

public class OsirionServiceCollectionExtensionsTests
{
    [Fact]
    public void AddOsirionCore_WithNullServices_ShouldThrowArgumentNullException()
    {
        // Arrange
        IServiceCollection? services = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => services!.AddOsirionCore());
    }

    [Fact]
    public void AddOsirionCore_WithValidServices_ShouldReturnServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddOsirionCore();

        // Assert
        result.ShouldBe(services);
    }
}