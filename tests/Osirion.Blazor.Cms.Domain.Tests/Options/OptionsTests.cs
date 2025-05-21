using Osirion.Blazor.Cms.Domain.Options;

namespace Osirion.Blazor.Cms.Domain.Tests.Domain.Options;

public class OptionsTests
{
    [Fact]
    public void ContentProviderOptions_Constructor_InitializesDefaultValues()
    {
        // Act
        var options = new ContentProviderOptions();

        // Assert
        Assert.Null(options.ProviderId);
        Assert.True(options.EnableCaching);
        Assert.Equal(30, options.CacheDurationMinutes);
        Assert.False(options.IsDefault);
        Assert.False(options.EnableLocalization);
        Assert.Equal("en", options.DefaultLocale);
        Assert.Single(options.SupportedLocales);
        Assert.Equal("en", options.SupportedLocales[0]);
        Assert.Equal(2, options.SupportedExtensions.Count);
        Assert.Contains(".md", options.SupportedExtensions);
        Assert.Contains(".markdown", options.SupportedExtensions);
        Assert.True(options.ValidateContent);
    }

    [Fact]
    public void GitHubOptions_Constructor_InitializesDefaultValues()
    {
        // Act
        var options = new GitHubOptions();

        // Assert
        // Inherits from ContentProviderOptions
        Assert.Null(options.ProviderId);
        Assert.True(options.EnableCaching);
        Assert.Equal(30, options.CacheDurationMinutes);
        Assert.False(options.IsDefault);

        // GitHubOptions specific properties
        Assert.Empty(options.Owner);
        Assert.Empty(options.Repository);
        Assert.Empty(options.ContentPath);
        Assert.Equal("main", options.Branch);
        Assert.Null(options.ApiToken);
        Assert.Equal("https://api.github.com", options.ApiUrl);
    }

    [Fact]
    public void FileSystemOptions_Constructor_InitializesDefaultValues()
    {
        // Act
        var options = new FileSystemOptions();

        // Assert
        // Inherits from ContentProviderOptions
        Assert.Null(options.ProviderId);
        Assert.True(options.EnableCaching);
        Assert.Equal(30, options.CacheDurationMinutes);
        Assert.False(options.IsDefault);

        // FileSystemOptions specific properties
        Assert.Empty(options.BasePath);
        Assert.True(options.WatchForChanges);
        Assert.Equal(30000, options.PollingIntervalMs);
        Assert.True(options.IncludeSubdirectories);
        Assert.False(options.CreateDirectoriesIfNotExist);
        Assert.Equal(2, options.IncludePatterns.Count);
        Assert.Contains("**/*.md", options.IncludePatterns);
        Assert.Contains("**/*.markdown", options.IncludePatterns);
        Assert.Equal(3, options.ExcludePatterns.Count);
        Assert.Contains("**/node_modules/**", options.ExcludePatterns);
        Assert.Contains("**/bin/**", options.ExcludePatterns);
        Assert.Contains("**/obj/**", options.ExcludePatterns);
        Assert.Null(options.ContentRoot);
    }

    [Fact]
    public void CacheOptions_Constructor_InitializesDefaultValues()
    {
        // Act
        var options = new CacheOptions();

        // Assert
        Assert.True(options.Enabled);
        Assert.Equal(5, options.StaleTimeMinutes);
        Assert.Equal(60, options.MaxAgeMinutes);
        Assert.True(options.UseStaleWhileRevalidate);
        Assert.Equal(64, options.SizeLimitMB);
        Assert.Equal(0.25, options.CompactionPercentage);
    }

    [Fact]
    public void GithubAuthorizationOptions_Constructor_InitializesDefaultValues()
    {
        // Act
        var options = new GithubAuthorizationOptions();

        // Assert
        Assert.Empty(options.ClientId);
        Assert.Empty(options.ClientSecret);
    }

    [Fact]
    public void SectionNames_AreCorrectlyDefined()
    {
        // Assert
        Assert.Equal("Osirion:Cms:Cache", CacheOptions.Section);
        Assert.Equal("Osirion:Cms:GitHub:Web", GitHubOptions.Section);
        Assert.Equal("Osirion:Cms:FileSystem:Web", FileSystemOptions.Section);
        Assert.Equal("Osirion:Cms:GitHub:Authorization", GithubAuthorizationOptions.Section);
    }
}