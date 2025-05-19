using Microsoft.Extensions.Logging;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Infrastructure.Decorators;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Decorators
{
    public class LoggingContentProviderDecoratorTests
    {
        private readonly IReadContentProvider _inner;
        private readonly ILogger<LoggingContentProviderDecorator> _logger;
        private readonly LoggingContentProviderDecorator _decorator;

        public LoggingContentProviderDecoratorTests()
        {
            _inner = Substitute.For<IReadContentProvider>();
            _logger = Substitute.For<ILogger<LoggingContentProviderDecorator>>();

            _decorator = new LoggingContentProviderDecorator(_inner, _logger);
        }

        [Fact]
        public async Task GetByIdAsync_LogsAndDelegates()
        {
            // Arrange
            var id = Guid.NewGuid();
            var contentItem = new ContentItem();
            _inner.GetByIdAsync(id).Returns(contentItem);

            // Act
            var result = await _decorator.GetByIdAsync(id);

            // Assert
            result.ShouldBeSameAs(contentItem);
            await _inner.Received(1).GetByIdAsync(id);

            // Verify logging
            _logger.Received(1).Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Is<object>(o => o.ToString().Contains($"Fetching content {id}")),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception, string>>());

            _logger.Received(1).Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Is<object>(o => o.ToString().Contains($"Fetched content {id}")),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception, string>>());
        }

        [Fact]
        public async Task GetAllAsync_LogsAndDelegates()
        {
            // Arrange
            var contentItems = new List<ContentItem>
            {
                new ContentItem(),
                new ContentItem()
            };
            _inner.GetAllAsync().Returns(contentItems);

            // Act
            var result = await _decorator.GetAllAsync();

            // Assert
            result.ShouldBeSameAs(contentItems);
            await _inner.Received(1).GetAllAsync();

            // Verify logging
            _logger.Received(1).Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Is<object>(o => o.ToString().Contains("Fetching all content")),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception, string>>());

            _logger.Received(1).Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Is<object>(o => o.ToString().Contains("Fetched 2 items")),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception, string>>());
        }
    }
}