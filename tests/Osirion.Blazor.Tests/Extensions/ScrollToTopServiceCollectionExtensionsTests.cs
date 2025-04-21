using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Extensions;
using Osirion.Blazor.Options;
using Shouldly;

namespace Osirion.Blazor.Tests.Extensions;

public class ScrollToTopServiceCollectionExtensionsTests
{
    [Fact]
    public void AddScrollToTop_WithConfiguration_ShouldConfigureOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ScrollToTop:VisibilityThreshold", "500" },
                { "ScrollToTop:Position", "TopRight" },
                { "ScrollToTop:Title", "Test Title" },
                { "ScrollToTop:Text", "Test Text" },
                { "ScrollToTop:UseStyles", "false" }
            })
            .Build();

        // Act
        services.AddScrollToTop(configuration);
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<ScrollToTopOptions>>().Value;

        // Assert
        options.VisibilityThreshold.ShouldBe(500);
        options.Position.ShouldBe(ButtonPosition.TopRight);
        options.Title.ShouldBe("Test Title");
        options.Text.ShouldBe("Test Text");
        options.UseStyles.ShouldBeFalse();
    }

    [Fact]
    public void AddScrollToTop_WithActionDelegate_ShouldConfigureOptions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddScrollToTop(options =>
        {
            options.VisibilityThreshold = 600;
            options.Position = ButtonPosition.BottomLeft;
            options.Title = "Delegate Title";
            options.Text = "Delegate Text";
        });
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<ScrollToTopOptions>>().Value;

        // Assert
        options.VisibilityThreshold.ShouldBe(600);
        options.Position.ShouldBe(ButtonPosition.BottomLeft);
        options.Title.ShouldBe("Delegate Title");
        options.Text.ShouldBe("Delegate Text");
        options.UseStyles.ShouldBeFalse();
    }

    [Fact]
    public void AddScrollToTop_WithNullServices_ShouldThrowArgumentNullException()
    {
        // Arrange
        IServiceCollection? services = null;
        var configuration = new ConfigurationBuilder().Build();

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => services!.AddScrollToTop(configuration));
    }
}