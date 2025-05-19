using Osirion.Blazor.Cms.Domain.Options.Configuration;
using Shouldly;

namespace Osirion.Blazor.Cms.Domain.Tests.Options;

public class CmsAdminOptionsTests
{
    [Fact]
    public void DefaultProperties_HaveExpectedValues()
    {
        // Arrange
        var options = new CmsAdminOptions();

        // Assert
        options.DefaultContentProvider.ShouldBe("GitHub");
        options.PersistUserSelections.ShouldBeTrue();
        options.GitHub.ShouldNotBeNull();
        options.FileSystem.ShouldNotBeNull();
        options.Authentication.ShouldNotBeNull();
        options.Theme.ShouldNotBeNull();
        options.ContentRules.ShouldNotBeNull();
        options.Localization.ShouldNotBeNull();
    }

    [Fact]
    public void Properties_CanBeSet_AndRetrieved()
    {
        // Arrange
        var options = new CmsAdminOptions
        {
            DefaultContentProvider = "FileSystem",
            PersistUserSelections = false
        };

        options.GitHub = new GitHubAdminOptions
        {
            Owner = "test-owner"
        };

        options.FileSystem = new FileSystemAdminOptions
        {
            RootPath = "/test/path"
        };

        // Assert
        options.DefaultContentProvider.ShouldBe("FileSystem");
        options.PersistUserSelections.ShouldBeFalse();
        options.GitHub.Owner.ShouldBe("test-owner");
        options.FileSystem.RootPath.ShouldBe("/test/path");
    }

    [Fact]
    public void ChildOptions_AreInstantiated()
    {
        // Arrange
        var options = new CmsAdminOptions();

        // Assert
        options.GitHub.ShouldBeOfType<GitHubAdminOptions>();
        options.FileSystem.ShouldBeOfType<FileSystemAdminOptions>();
        options.Authentication.ShouldBeOfType<AuthenticationOptions>();
        options.Theme.ShouldBeOfType<ThemeOptions>();
        options.ContentRules.ShouldBeOfType<ContentRulesOptions>();
        options.Localization.ShouldBeOfType<LocalizationOptions>();
    }
}