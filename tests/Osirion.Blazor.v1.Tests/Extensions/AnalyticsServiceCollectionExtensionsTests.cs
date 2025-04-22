using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Components.Analytics.Options;
using Osirion.Blazor.Extensions;

namespace Osirion.Blazor.Tests.Extensions;

public class AnalyticsServiceCollectionExtensionsTests
{
    [Fact]
    public void AddClarityTracker_WithConfiguration_ShouldConfigureOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Clarity:TrackerUrl", "https://test.clarity.ms/tag/" },
                { "Clarity:SiteId", "test-site-id" },
                { "Clarity:Track", "true" }
            })
            .Build();

        // Act
        services.AddClarityTracker(configuration);
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<ClarityOptions>>().Value;

        // Assert
        Assert.Equal("https://test.clarity.ms/tag/", options.TrackerUrl);
        Assert.Equal("test-site-id", options.SiteId);
        Assert.True(options.Track);
    }

    [Fact]
    public void AddClarityTracker_WithActionDelegate_ShouldConfigureOptions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddClarityTracker(options =>
        {
            options.TrackerUrl = "https://test.clarity.ms/tag/";
            options.SiteId = "test-site-id";
            options.Track = false;
        });
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<ClarityOptions>>().Value;

        // Assert
        Assert.Equal("https://test.clarity.ms/tag/", options.TrackerUrl);
        Assert.Equal("test-site-id", options.SiteId);
        Assert.False(options.Track);
    }

    [Fact]
    public void AddClarityTracker_WithNullServices_ShouldThrowArgumentNullException()
    {
        // Arrange
        IServiceCollection? services = null;
        var configuration = new ConfigurationBuilder().Build();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => services!.AddClarityTracker(configuration));
    }

    [Fact]
    public void AddClarityTracker_WithNullConfiguration_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        IConfiguration? configuration = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => services.AddClarityTracker(configuration!));
    }

    [Fact]
    public void AddClarityTracker_WithNullActionDelegate_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        Action<ClarityOptions>? configureOptions = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => services.AddClarityTracker(configureOptions!));
    }

    [Fact]
    public void AddMatomoTracker_WithConfiguration_ShouldConfigureOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Matomo:TrackerUrl", "//test.matomo.com/" },
                { "Matomo:SiteId", "123" },
                { "Matomo:Track", "true" }
            })
            .Build();

        // Act
        services.AddMatomoTracker(configuration);
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<MatomoOptions>>().Value;

        // Assert
        Assert.Equal("//test.matomo.com/", options.TrackerUrl);
        Assert.Equal("123", options.SiteId);
        Assert.True(options.Track);
    }

    [Fact]
    public void AddMatomoTracker_WithActionDelegate_ShouldConfigureOptions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMatomoTracker(options =>
        {
            options.TrackerUrl = "//test.matomo.com/";
            options.SiteId = "456";
            options.Track = false;
        });
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<MatomoOptions>>().Value;

        // Assert
        Assert.Equal("//test.matomo.com/", options.TrackerUrl);
        Assert.Equal("456", options.SiteId);
        Assert.False(options.Track);
    }

    [Fact]
    public void AddMatomoTracker_WithNullServices_ShouldThrowArgumentNullException()
    {
        // Arrange
        IServiceCollection? services = null;
        var configuration = new ConfigurationBuilder().Build();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => services!.AddMatomoTracker(configuration));
    }

    [Fact]
    public void AddMatomoTracker_WithNullConfiguration_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        IConfiguration? configuration = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => services.AddMatomoTracker(configuration!));
    }

    [Fact]
    public void AddMatomoTracker_WithNullActionDelegate_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        Action<MatomoOptions>? configureOptions = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => services.AddMatomoTracker(configureOptions!));
    }
}