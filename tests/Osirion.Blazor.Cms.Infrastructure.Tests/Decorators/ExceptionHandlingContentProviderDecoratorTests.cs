using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Infrastructure.Decorators;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Decorators;

public class ExceptionHandlingContentProviderDecoratorTests
{
    private readonly IReadContentProvider _inner;
    private readonly ExceptionHandlingContentProviderDecorator _decorator;

    public ExceptionHandlingContentProviderDecoratorTests()
    {
        _inner = Substitute.For<IReadContentProvider>();
        _decorator = new ExceptionHandlingContentProviderDecorator(_inner);
    }

    [Fact]
    public async Task GetByIdAsync_WhenInnerSucceeds_ReturnsContentItem()
    {
        // Arrange
        var id = Guid.NewGuid();
        var contentItem = new ContentItem();
        _inner.GetByIdAsync(id).Returns(contentItem);

        // Act
        var result = await _decorator.GetByIdAsync(id);

        // Assert
        result.ShouldBe(contentItem);
        await _inner.Received(1).GetByIdAsync(id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenInnerThrows_WrapsExceptionInContentProviderException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var innerException = new Exception("Test error");
        _inner.GetByIdAsync(id).Throws(innerException);

        // Act & Assert
        var ex = await Should.ThrowAsync<ContentProviderException>(async () =>
            await _decorator.GetByIdAsync(id));

        ex.Message.ShouldContain($"Error fetching {id}");
        ex.InnerException.ShouldBe(innerException);
    }

    [Fact]
    public async Task GetAllAsync_WhenInnerSucceeds_ReturnsContentItems()
    {
        // Arrange
        var contentItems = new List<ContentItem> { new ContentItem(), new ContentItem() };
        _inner.GetAllAsync().Returns(contentItems);

        // Act
        var result = await _decorator.GetAllAsync();

        // Assert
        result.ShouldBe(contentItems);
        await _inner.Received(1).GetAllAsync();
    }

    [Fact]
    public async Task GetAllAsync_WhenInnerThrows_WrapsExceptionInContentProviderException()
    {
        // Arrange
        var innerException = new Exception("Test error");
        _inner.GetAllAsync().Throws(innerException);

        // Act & Assert
        var ex = await Should.ThrowAsync<ContentProviderException>(async () =>
            await _decorator.GetAllAsync());

        ex.Message.ShouldContain("Error fetching all content");
        ex.InnerException.ShouldBe(innerException);
    }

    [Fact]
    public void Constructor_WithNullInner_ThrowsArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() =>
            new ExceptionHandlingContentProviderDecorator(null))
            .ParamName.ShouldBe("inner");
    }

    [Fact]
    public void Constructor_InitializesInnerProperty()
    {
        // Arrange
        var decorator = new ExceptionHandlingContentProviderDecorator(_inner);

        // Assert - Check _inner field via reflection
        var innerField = typeof(ExceptionHandlingContentProviderDecorator)
            .GetField("_inner", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        innerField.ShouldNotBeNull();
        var innerValue = innerField.GetValue(decorator);
        innerValue.ShouldBe(_inner);
    }

    [Fact]
    public async Task GetByIdAsync_WithContentProviderException_DoesNotWrapAgain()
    {
        // Arrange
        var id = Guid.NewGuid();
        var originalException = new ContentProviderException("Original error");
        _inner.GetByIdAsync(id).Throws(originalException);

        // Act & Assert
        var ex = await Should.ThrowAsync<ContentProviderException>(async () =>
            await _decorator.GetByIdAsync(id));

        ex.ShouldBe(originalException);
    }

    [Fact]
    public async Task GetAllAsync_WithContentProviderException_DoesNotWrapAgain()
    {
        // Arrange
        var originalException = new ContentProviderException("Original error");
        _inner.GetAllAsync().Throws(originalException);

        // Act & Assert
        var ex = await Should.ThrowAsync<ContentProviderException>(async () =>
            await _decorator.GetAllAsync());

        ex.ShouldBe(originalException);
    }
}