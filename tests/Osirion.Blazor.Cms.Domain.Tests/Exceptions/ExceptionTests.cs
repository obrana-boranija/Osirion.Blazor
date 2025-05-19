using Osirion.Blazor.Cms.Domain.Exceptions;

namespace Osirion.Blazor.Cms.Domain.Tests.Exceptions;

public class ExceptionTests
{
    [Fact]
    public void DomainException_WithMessage_SetsMessage()
    {
        // Arrange
        string message = "Test domain exception";

        // Act
        var exception = new TestDomainException(message);

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void DomainException_WithMessageAndInnerException_SetsBoth()
    {
        // Arrange
        string message = "Test domain exception";
        var innerException = new InvalidOperationException("Inner exception");

        // Act
        var exception = new TestDomainException(message, innerException);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Same(innerException, exception.InnerException);
    }

    [Fact]
    public void ContentItemNotFoundException_SetsProperties()
    {
        // Arrange
        string contentId = "missing-content";
        string providerType = "test-provider";
        string expectedMessage = $"Content item with ID '{contentId}' was not found in provider '{providerType}'.";

        // Act
        var exception = new ContentItemNotFoundException(contentId, providerType);

        // Assert
        Assert.Equal(contentId, exception.ContentId);
        Assert.Equal(providerType, exception.ProviderType);
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void ContentItemNotFoundException_WithoutProviderType_SetsMessageCorrectly()
    {
        // Arrange
        string contentId = "missing-content";
        string expectedMessage = $"Content item with ID '{contentId}' was not found.";

        // Act
        var exception = new ContentItemNotFoundException(contentId);

        // Assert
        Assert.Equal(contentId, exception.ContentId);
        Assert.Null(exception.ProviderType);
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void DirectoryNotFoundException_SetsProperties()
    {
        // Arrange
        string directoryId = "missing-directory";
        string expectedMessage = $"Directory with ID '{directoryId}' was not found.";

        // Act
        var exception = new Cms.Domain.Exceptions.DirectoryNotFoundException(directoryId);

        // Assert
        Assert.Equal(directoryId, exception.DirectoryId);
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void ContentProviderException_WithMessage_SetsMessage()
    {
        // Arrange
        string message = "Provider error";
        string providerId = "test-provider";

        // Act
        var exception = new ContentProviderException(message, providerId);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Equal(providerId, exception.ProviderId);
    }

    [Fact]
    public void ContentProviderException_WithInnerException_SetsInnerException()
    {
        // Arrange
        string message = "Provider error";
        string providerId = "test-provider";
        var innerException = new InvalidOperationException("Inner exception");

        // Act
        var exception = new ContentProviderException(message, innerException, providerId);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Equal(providerId, exception.ProviderId);
        Assert.Same(innerException, exception.InnerException);
    }

    [Fact]
    public void ContentValidationException_WithMessageAndErrorsDictionary_SetsProperties()
    {
        // Arrange
        string message = "Validation failed";
        var errors = new Dictionary<string, string[]>
        {
            { "Title", new[] { "Title is required" } },
            { "Content", new[] { "Content is too short", "Content has invalid format" } }
        };

        // Act
        var exception = new ContentValidationException(message, errors);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Equal(2, exception.Errors.Count);
        Assert.Equal(errors, exception.Errors);
    }

    [Fact]
    public void ContentValidationException_WithPropertyAndErrorMessage_SetsErrorsDictionary()
    {
        // Arrange
        string propertyName = "Title";
        string errorMessage = "Title is required";
        string expectedMessage = $"Validation failed: {propertyName} - {errorMessage}";

        // Act
        var exception = new ContentValidationException(propertyName, errorMessage);

        // Assert
        Assert.Equal(expectedMessage, exception.Message);
        Assert.Single(exception.Errors);
        Assert.True(exception.Errors.ContainsKey(propertyName));
        Assert.Equal(new[] { errorMessage }, exception.Errors[propertyName]);
    }

    // Test implementation of abstract DomainException
    private class TestDomainException : DomainException
    {
        public TestDomainException(string message) : base(message) { }
        public TestDomainException(string message, Exception innerException) : base(message, innerException) { }
    }
}