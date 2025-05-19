using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Domain.Tests.Models.GitHub;

public class GitHubApiResponseTests
{
    [Fact]
    public void GitHubApiResponse_DefaultValues_PropertiesHaveExpectedDefaults()
    {
        // Arrange & Act
        var response = new TestGitHubApiResponse();

        // Assert
        Assert.False(response.Success);
        Assert.Null(response.ErrorMessage);
    }

    [Fact]
    public void GitHubApiResponse_WithSuccessTrue_SuccessIsTrue()
    {
        // Arrange
        var response = new TestGitHubApiResponse
        {
            Success = true
        };

        // Act & Assert
        Assert.True(response.Success);
    }

    [Fact]
    public void GitHubApiResponse_WithErrorMessage_ErrorMessageIsSet()
    {
        // Arrange
        var errorMessage = "Not Found";

        // Act
        var response = new TestGitHubApiResponse
        {
            Success = false,
            ErrorMessage = errorMessage
        };

        // Assert
        Assert.False(response.Success);
        Assert.Equal(errorMessage, response.ErrorMessage);
    }

    [Theory]
    [InlineData(true, null)]
    [InlineData(false, "Unauthorized")]
    [InlineData(true, "Warning message")]
    public void GitHubApiResponse_WithVariousValues_PropertiesAreSetCorrectly(bool success, string? errorMessage)
    {
        // Arrange & Act
        var response = new TestGitHubApiResponse
        {
            Success = success,
            ErrorMessage = errorMessage
        };

        // Assert
        Assert.Equal(success, response.Success);
        Assert.Equal(errorMessage, response.ErrorMessage);
    }

    // TestGitHubApiResponse - concrete implementation of the abstract GitHubApiResponse class for testing
    private class TestGitHubApiResponse : GitHubApiResponse
    {
    }
}