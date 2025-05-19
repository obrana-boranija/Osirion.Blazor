using Microsoft.Playwright;

namespace Osirion.Blazor.E2ETests.Tests;

/// <summary>
/// Tests for Osirion CMS components
/// </summary>
public class CmsComponentTests : IClassFixture<PlaywrightFixture>
{
    private readonly PlaywrightFixture _fixture;
    private readonly TestApp _app;

    public CmsComponentTests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
        _app = new TestApp(fixture);
    }

    [Fact]
    public async Task ContentList_ShouldRenderItemsCorrectly()
    {
        // Arrange
        await _app.NavigateToComponentAsync("contentlist");

        // Act
        await _app.WaitForComponentAsync(".osirion-content-list");

        // Assert
        // Check if ContentList is rendered
        var contentList = await _fixture.Page.QuerySelectorAsync(".osirion-content-list");
        Assert.NotNull(contentList);

        // Check if ContentList has items
        var contentItems = await _fixture.Page.QuerySelectorAllAsync(".osirion-content-card");
        Assert.True(contentItems.Count > 0, "ContentList should render at least one content item");

        // Check item structure
        var firstItem = contentItems[0];
        var title = await firstItem.QuerySelectorAsync(".osirion-content-title");
        Assert.NotNull(title);
        Assert.True(title != null, "Content item should have a title");

        var description = await firstItem.QuerySelectorAsync(".osirion-content-description");
        Assert.NotNull(description);
        Assert.True(description != null, "Content item should have a description");

        var readMoreLink = await firstItem.QuerySelectorAsync(".osirion-content-read-more");
        Assert.NotNull(readMoreLink);
        Assert.True(readMoreLink != null, "Content item should have a read more link");

        // Take screenshot
        await _app.TakeScreenshotAsync("ContentList_Rendered");
    }

    [Fact]
    public async Task ContentView_ShouldRenderContentCorrectly()
    {
        // Arrange
        await _app.NavigateToComponentAsync("contentview");

        // Act
        await _app.WaitForComponentAsync(".osirion-content-view");

        // Assert
        // Check if ContentView is rendered
        var contentView = await _fixture.Page.QuerySelectorAsync(".osirion-content-view");
        Assert.NotNull(contentView);

        // Check if content is properly rendered
        var article = await _fixture.Page.QuerySelectorAsync(".osirion-content-article");
        Assert.NotNull(article);
        Assert.True(article != null, "ContentView should render an article element");

        // Check content structure
        var title = await article.QuerySelectorAsync("h1");
        Assert.NotNull(title);
        Assert.True(title != null, "Article should have a title (h1)");

        var content = await article.QuerySelectorAsync("p");
        Assert.NotNull(content);
        Assert.True(content != null, "Article should have content paragraphs");

        // Take screenshot
        await _app.TakeScreenshotAsync("ContentView_Rendered");
    }

    [Fact]
    public async Task TagCloud_ShouldRenderAndRespond()
    {
        // Arrange
        await _app.NavigateToComponentAsync("tagcloud");

        // Act
        await _app.WaitForComponentAsync(".osirion-tag-cloud");

        // Assert
        // Check if TagCloud is rendered
        var tagCloud = await _fixture.Page.QuerySelectorAsync(".osirion-tag-cloud");
        Assert.NotNull(tagCloud);

        // Check if tags are rendered
        var tags = await _fixture.Page.QuerySelectorAllAsync(".osirion-tag-link");
        Assert.True(tags.Count > 0, "TagCloud should render at least one tag");

        // Test interaction - Click a tag
        await tags[0].ClickAsync();

        // Wait for navigation or tag selection
        await _fixture.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Check if tag is selected
        var activeTag = await _fixture.Page.QuerySelectorAsync(".osirion-tag-active");
        Assert.NotNull(activeTag);
        Assert.True(activeTag != null, "A tag should be activated after clicking");

        // Take screenshot
        await _app.TakeScreenshotAsync("TagCloud_WithSelection");
    }

    [Fact]
    public async Task CategoriesList_ShouldRenderAndRespond()
    {
        // Arrange
        await _app.NavigateToComponentAsync("categorieslist");

        // Act
        await _app.WaitForComponentAsync(".osirion-categories-list-container");

        // Assert
        // Check if CategoriesList is rendered
        var categoriesList = await _fixture.Page.QuerySelectorAsync(".osirion-categories-list-container");
        Assert.NotNull(categoriesList);

        // Check if categories are rendered
        var categories = await _fixture.Page.QuerySelectorAllAsync(".osirion-category-link");
        Assert.True(categories.Count > 0, "CategoriesList should render at least one category");

        // Test interaction - Click a category
        await categories[0].ClickAsync();

        // Wait for navigation or category selection
        await _fixture.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Check if category is selected
        var activeCategory = await _fixture.Page.QuerySelectorAsync(".osirion-active");
        Assert.NotNull(activeCategory);
        Assert.True(activeCategory != null, "A category should be activated after clicking");

        // Take screenshot
        await _app.TakeScreenshotAsync("CategoriesList_WithSelection");
    }

    [Fact]
    public async Task SearchBox_ShouldSubmitSearchQuery()
    {
        // Arrange
        await _app.NavigateToComponentAsync("searchbox");

        // Act
        await _app.WaitForComponentAsync(".osirion-search-box");

        // Fill search input
        await _fixture.Page.FillAsync(".osirion-search-input", "test query");

        // Submit search
        await _fixture.Page.ClickAsync(".osirion-search-button");

        // Wait for search results
        await _fixture.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Assert
        // Check URL contains search parameter
        var url = _fixture.Page.Url;
        Assert.Contains("search=test+query", url);

        // Take screenshot
        await _app.TakeScreenshotAsync("SearchBox_QuerySubmitted");
    }

    [Fact]
    public async Task DirectoryNavigation_ShouldRenderAndNavigate()
    {
        // Arrange
        await _app.NavigateToComponentAsync("directorynavigation");

        // Act
        await _app.WaitForComponentAsync(".osirion-directory-navigation");

        // Assert
        // Check if DirectoryNavigation is rendered
        var directoryNavigation = await _fixture.Page.QuerySelectorAsync(".osirion-directory-navigation");
        Assert.NotNull(directoryNavigation);

        // Check if directories are rendered
        var directories = await _fixture.Page.QuerySelectorAllAsync(".osirion-directory-link");
        Assert.True(directories.Count > 0, "DirectoryNavigation should render at least one directory");

        // Test interaction - Click a directory
        await directories[0].ClickAsync();

        // Wait for navigation
        await _fixture.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Check if directory is selected
        var activeDirectory = await _fixture.Page.QuerySelectorAsync(".osirion-directory-link.osirion-active");
        Assert.NotNull(activeDirectory);
        Assert.True(activeDirectory != null, "A directory should be activated after clicking");

        // Take screenshot
        await _app.TakeScreenshotAsync("DirectoryNavigation_Selected");
    }
}