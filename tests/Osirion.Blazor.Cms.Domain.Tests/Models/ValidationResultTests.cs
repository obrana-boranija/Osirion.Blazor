using Osirion.Blazor.Cms.Domain.Models;

namespace Osirion.Blazor.Cms.Domain.Tests.Models;

public class ValidationResultTests
{
    [Fact]
    public void Constructor_DefaultValues_ShouldBeValid()
    {
        // Arrange & Act
        var result = new ValidationResult();

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void AddError_SingleError_ShouldMakeResultInvalid()
    {
        // Arrange
        var result = new ValidationResult();

        // Act
        result.AddError("Title", "Title is required");

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Single(result.Errors["Title"]);
        Assert.Equal("Title is required", result.Errors["Title"][0]);
    }

    [Fact]
    public void AddError_MultipleErrorsForSameProperty_ShouldAddAll()
    {
        // Arrange
        var result = new ValidationResult();

        // Act
        result.AddError("Title", "Title is required");
        result.AddError("Title", "Title must be unique");

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal(2, result.Errors["Title"].Count);
        Assert.Equal("Title is required", result.Errors["Title"][0]);
        Assert.Equal("Title must be unique", result.Errors["Title"][1]);
    }

    [Fact]
    public void AddError_MultipleProperties_ShouldAddAll()
    {
        // Arrange
        var result = new ValidationResult();

        // Act
        result.AddError("Title", "Title is required");
        result.AddError("Content", "Content is too short");

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(2, result.Errors.Count);
        Assert.Single(result.Errors["Title"]);
        Assert.Single(result.Errors["Content"]);
        Assert.Equal("Title is required", result.Errors["Title"][0]);
        Assert.Equal("Content is too short", result.Errors["Content"][0]);
    }

    [Fact]
    public void GetAllErrors_SingleError_ShouldReturnFormattedError()
    {
        // Arrange
        var result = new ValidationResult();
        result.AddError("Title", "Title is required");

        // Act
        var errors = result.GetAllErrors();

        // Assert
        Assert.Single(errors);
        Assert.Equal("Title: Title is required", errors[0]);
    }

    [Fact]
    public void GetAllErrors_MultipleErrors_ShouldReturnAllFormattedErrors()
    {
        // Arrange
        var result = new ValidationResult();
        result.AddError("Title", "Title is required");
        result.AddError("Title", "Title must be unique");
        result.AddError("Content", "Content is too short");

        // Act
        var errors = result.GetAllErrors();

        // Assert
        Assert.Equal(3, errors.Count);
        Assert.Contains("Title: Title is required", errors);
        Assert.Contains("Title: Title must be unique", errors);
        Assert.Contains("Content: Content is too short", errors);
    }

    [Fact]
    public void Success_StaticMethod_ShouldReturnValidResult()
    {
        // Arrange & Act
        var result = ValidationResult.Success();

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Failure_WithSingleError_ShouldReturnInvalidResult()
    {
        // Arrange & Act
        var result = ValidationResult.Failure("Title", "Title is required");

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Single(result.Errors["Title"]);
        Assert.Equal("Title is required", result.Errors["Title"][0]);
    }

    [Fact]
    public void Failure_WithMultipleErrors_ShouldReturnInvalidResult()
    {
        // Arrange
        var errors = new Dictionary<string, List<string>>
        {
            { "Title", new List<string> { "Title is required", "Title must be unique" } },
            { "Content", new List<string> { "Content is too short" } }
        };

        // Act
        var result = ValidationResult.Failure(errors);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(2, result.Errors.Count);
        Assert.Equal(2, result.Errors["Title"].Count);
        Assert.Single(result.Errors["Content"]);
    }
}