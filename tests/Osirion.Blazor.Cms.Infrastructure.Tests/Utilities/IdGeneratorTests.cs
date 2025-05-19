using Osirion.Blazor.Cms.Infrastructure.Utilities;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Utilities
{
    public class IdGeneratorTests
    {
        [Fact]
        public void GenerateStableId_WithValidPath_ReturnsConsistentHash()
        {
            // Arrange
            var path = "content/blog/post-1.md";

            // Act
            var id1 = IdGenerator.GenerateStableId(path);
            var id2 = IdGenerator.GenerateStableId(path);

            // Assert
            id1.ShouldNotBeNullOrEmpty();
            id1.ShouldBe(id2); // Same input should yield same output
        }

        [Fact]
        public void GenerateStableId_WithAdditionalComponents_IncorporatesAllComponents()
        {
            // Arrange
            var path = "content/blog/post-1.md";
            var providerId = "github-test";

            // Act
            var idWithoutProvider = IdGenerator.GenerateStableId(path);
            var idWithProvider = IdGenerator.GenerateStableId(path, providerId);

            // Assert
            idWithoutProvider.ShouldNotBe(idWithProvider); // Different inputs should yield different outputs
        }

        [Fact]
        public void GenerateDirectoryId_WithValidInputs_ReturnsConsistentId()
        {
            // Arrange
            var path = "content/blog";
            var name = "Blog";
            var providerId = "github-test";

            // Act
            var directoryId = IdGenerator.GenerateDirectoryId(path, name, providerId);

            // Assert
            directoryId.ShouldNotBeNullOrEmpty();
            directoryId.ShouldBe(IdGenerator.GenerateStableId(path, name, providerId));
        }

        [Fact]
        public void GenerateContentId_WithValidInputs_ReturnsConsistentId()
        {
            // Arrange
            var path = "content/blog/post-1.md";
            var title = "My First Post";
            var providerId = "github-test";

            // Act
            var contentId = IdGenerator.GenerateContentId(path, title, providerId);

            // Assert
            contentId.ShouldNotBeNullOrEmpty();
            contentId.ShouldBe(IdGenerator.GenerateStableId(path, title, providerId));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void GenerateStableId_WithInvalidPath_ThrowsArgumentException(string invalidPath)
        {
            // Act & Assert
            Should.Throw<ArgumentException>(() => IdGenerator.GenerateStableId(invalidPath));
        }
    }
}