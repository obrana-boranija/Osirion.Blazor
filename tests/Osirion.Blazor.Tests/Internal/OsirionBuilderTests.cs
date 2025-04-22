using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Tests.Fakes;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osirion.Blazor.Tests.Internal;

public class OsirionBuilderTests
{
    [Fact]
    public void Constructor_WithNullServices_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Should.Throw<ArgumentNullException>(() => new TestOsirionBuilder(null!));
    }

    [Fact]
    public void Builder_ShouldImplementIOsirionBuilder()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var builder = new TestOsirionBuilder(services);

        // Assert
        builder.ShouldBeAssignableTo<IOsirionBuilder>();
        builder.Services.ShouldBe(services);
    }

    [Fact]
    public void UseContent_WithNullConfigure_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new TestOsirionBuilder(services);

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => builder.UseContent(null!));
    }

    [Fact]
    public void UseAnalytics_WithNullConfigure_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new TestOsirionBuilder(services);

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => builder.UseAnalytics(null!));
    }

    [Fact]
    public void UseNavigation_WithNullConfigure_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new TestOsirionBuilder(services);

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => builder.UseNavigation(null!));
    }

    [Fact]
    public void UseTheming_WithNullConfigure_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new TestOsirionBuilder(services);

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => builder.UseTheming(null!));
    }

    [Fact]
    public void UseContent_ShouldDelegateToContentExtensions()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new TestOsirionBuilder(services);
        var configureCalled = false;

        // Act
        builder.UseContent(content =>
        {
            configureCalled = true;
            content.ShouldNotBeNull();
        });

        // Assert
        configureCalled.ShouldBeTrue();
    }
}