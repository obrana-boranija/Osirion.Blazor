using Osirion.Blazor.Cms.Domain.Options.Configuration;
using Shouldly;

namespace Osirion.Blazor.Cms.Domain.Tests.Options;

public class AuthenticationOptionsTests
{
    [Fact]
    public void DefaultProperties_HaveExpectedValues()
    {
        // Arrange
        var options = new AuthenticationOptions();

        // Assert
        options.EnableGitHubOAuth.ShouldBeFalse();
        options.GitHubClientId.ShouldBe(string.Empty);
        options.GitHubClientSecret.ShouldBe(string.Empty);
        options.GitHubRedirectUri.ShouldBe(string.Empty);
        options.AllowPersonalAccessTokens.ShouldBeTrue();
        options.PersonalAccessToken.ShouldBeNull();
    }

    [Fact]
    public void Properties_CanBeSet_AndRetrieved()
    {
        // Arrange
        var options = new AuthenticationOptions
        {
            EnableGitHubOAuth = true,
            GitHubClientId = "test-client-id",
            GitHubClientSecret = "test-client-secret",
            GitHubRedirectUri = "https://example.com/callback",
            AllowPersonalAccessTokens = false,
            PersonalAccessToken = "test-token"
        };

        // Assert
        options.EnableGitHubOAuth.ShouldBeTrue();
        options.GitHubClientId.ShouldBe("test-client-id");
        options.GitHubClientSecret.ShouldBe("test-client-secret");
        options.GitHubRedirectUri.ShouldBe("https://example.com/callback");
        options.AllowPersonalAccessTokens.ShouldBeFalse();
        options.PersonalAccessToken.ShouldBe("test-token");
    }
}