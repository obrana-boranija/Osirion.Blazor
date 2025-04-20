using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Components.GitHubCms;
using Osirion.Blazor.Models.Cms;
using Osirion.Blazor.Services.GitHub;
using Shouldly;


namespace Osirion.Blazor.Tests.Components.GitHubCms;

public class ContentListTests : TestContext
{
    [Fact]
    public void RendersLoadingState_WhenIsLoadingIsTrue()
    {
        // Arrange
        var cmsService = Substitute.For<IGitHubCmsService>();
        Services.AddSingleton(cmsService);

        // Act
        var cut = RenderComponent<ContentList>();

        // Assert
        cut.Markup.ShouldContain("Loading content...");
    }

    [Fact]
    public void RendersNoContentMessage_WhenContentItemsIsEmpty()
    {
        // Arrange
        var cmsService = Substitute.For<IGitHubCmsService>();
        cmsService.GetAllContentItemsAsync().Returns(Task.FromResult(new List<ContentItem>()));
        Services.AddSingleton(cmsService);

        // Act
        var cut = RenderComponent<ContentList>();

        // Wait for async tasks to complete
        cut.WaitForState(() => !cut.Instance.IsLoading);

        // Assert
        cut.Markup.ShouldContain("No content available.");
    }

    [Fact]
    public void RendersContentGrid_WhenContentItemsAreAvailable()
    {
        // Arrange
        var cmsService = Substitute.For<IGitHubCmsService>();
        var contentItems = new List<ContentItem>
            {
                new ContentItem
                {
                    Title = "Test Title",
                    Author = "Test Author",
                    Date = DateTime.Now,
                    Description = "Test Description",
                    Tags = new List<string> { "Tag1", "Tag2" },
                    FeaturedImageUrl = "https://example.com/image.jpg",
                    ReadTimeMinutes = 5
                }
            };
        cmsService.GetAllContentItemsAsync().Returns(Task.FromResult(contentItems));
        Services.AddSingleton(cmsService);

        // Act
        var cut = RenderComponent<ContentList>();

        // Wait for async tasks to complete
        cut.WaitForState(() => !cut.Instance.IsLoading);

        // Assert
        cut.Markup.ShouldContain("Test Title");
        cut.Markup.ShouldContain("Test Author");
        cut.Markup.ShouldContain("Test Description");
        cut.Markup.ShouldContain("Tag1");
        cut.Markup.ShouldContain("Tag2");
    }

    [Fact]
    public async Task LoadContentAsync_CallsCorrectServiceMethod_BasedOnParameters()
    {
        // Arrange
        var cmsService = Substitute.For<IGitHubCmsService>();
        Services.AddSingleton(cmsService);

        var cut = RenderComponent<ContentList>(parameters => parameters
            .Add(p => p.Directory, "test-directory")
        );

        // Act
        await cut.Instance.LoadContentAsync();

        // Assert
        await cmsService.Received(1).GetContentItemsByDirectoryAsync("test-directory");
    }

    [Fact]
    public async Task LoadContentAsync_HandlesExceptionsGracefully()
    {
        // Arrange
        var cmsService = Substitute.For<IGitHubCmsService>();
        cmsService.GetAllContentItemsAsync().Throws(new Exception("Test exception"));
        Services.AddSingleton(cmsService);

        var cut = RenderComponent<ContentList>();

        // Act
        await cut.Instance.LoadContentAsync();

        // Assert
        cut.Instance.IsLoading.ShouldBeFalse();
        cut.Instance.ContentItems.ShouldBeEmpty();
    }
}
