using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Infrastructure.Content;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Content
{
    public class ContentMetadataProcessorTests
    {
        private readonly ContentMetadataProcessor _processor;

        public ContentMetadataProcessorTests()
        {
            _processor = new ContentMetadataProcessor();
        }

        [Fact]
        public void ApplyMetadataToContentItem_SetsBasicProperties()
        {
            // Arrange
            var frontMatter = new Dictionary<string, string>
            {
                { "title", "Test Title" },
                { "author", "John Doe" },
                { "description", "Test description" },
                { "date", "2023-05-15" },
                { "slug", "test-slug" }
            };

            var contentItem = ContentItem.Create(
                "test-id",
                "Original Title",
                "Test content",
                "test.md",
                "test-provider");

            // Act
            var result = _processor.ApplyMetadataToContentItem(frontMatter, contentItem);

            // Assert
            result.Title.ShouldBe("Test Title");
            result.Author.ShouldBe("John Doe");
            result.Description.ShouldBe("Test description");
            result.DateCreated.ToString("yyyy-MM-dd").ShouldBe("2023-05-15");
            result.Slug.ShouldBe("test-slug");
        }

        [Fact]
        public void ApplyMetadataToContentItem_HandlesCategoriesAndTags()
        {
            // Arrange
            var frontMatter = new Dictionary<string, string>
            {
                { "categories", "Category1, Category2" },
                { "tags", "tag1, tag2, tag3" }
            };

            var contentItem = ContentItem.Create(
                "test-id",
                "Test Title",
                "Test content",
                "test.md",
                "test-provider");

            // Act
            var result = _processor.ApplyMetadataToContentItem(frontMatter, contentItem);

            // Assert
            result.Categories.Count.ShouldBe(2);
            result.Categories.ShouldContain("Category1");
            result.Categories.ShouldContain("Category2");

            result.Tags.Count.ShouldBe(3);
            result.Tags.ShouldContain("tag1");
            result.Tags.ShouldContain("tag2");
            result.Tags.ShouldContain("tag3");
        }

        [Fact]
        public void ApplyMetadataToContentItem_HandlesCustomMetadata()
        {
            // Arrange
            var frontMatter = new Dictionary<string, string>
            {
                { "custom_string", "string value" },
                { "custom_int", "42" },
                { "custom_bool", "true" },
                { "custom_double", "3.14" }
            };

            var contentItem = ContentItem.Create(
                "test-id",
                "Test Title",
                "Test content",
                "test.md",
                "test-provider");

            // Act
            var result = _processor.ApplyMetadataToContentItem(frontMatter, contentItem);

            // Assert
            result.Metadata.Count.ShouldBe(4);
            result.Metadata["custom_string"].ShouldBe("string value");
            result.Metadata["custom_int"].ShouldBe(42);
            result.Metadata["custom_bool"].ShouldBe(true);
            result.Metadata["custom_double"].ShouldBe(314);
        }

        [Fact]
        public void GenerateFrontMatter_CreatesCorrectYaml()
        {
            // Arrange
            var contentItem = ContentItem.Create(
                "test-id",
                "Test Title",
                "Test content",
                "test.md",
                "test-provider");

            contentItem.SetAuthor("John Doe");
            contentItem.SetDescription("Test description");
            contentItem.SetCreatedDate(new DateTime(2023, 5, 15));
            contentItem.SetSlug("test-slug");
            contentItem.AddCategory("Category1");
            contentItem.AddCategory("Category2");
            contentItem.AddTag("tag1");
            contentItem.AddTag("tag2");
            contentItem.SetFeatured(true);

            // Act
            var result = _processor.GenerateFrontMatter(contentItem);

            // Assert
            result.ShouldContain("title: \"Test Title\"");
            result.ShouldContain("author: \"John Doe\"");
            result.ShouldContain("description: \"Test description\"");
            result.ShouldContain("date: 2023-05-15");
            result.ShouldContain("slug: \"test-slug\"");
            result.ShouldContain("featured: true");
            result.ShouldContain("categories:");
            result.ShouldContain("  - \"Category1\"");
            result.ShouldContain("  - \"Category2\"");
            result.ShouldContain("tags:");
            result.ShouldContain("  - \"tag1\"");
            result.ShouldContain("  - \"tag2\"");
        }

        [Fact]
        public void ParseListValue_HandlesVariousFormats()
        {
            // Arrange & Act
            var commaSeparated = _processor.ParseListValue("item1, item2, item3").ToList();
            var semiColonSeparated = _processor.ParseListValue("item1; item2; item3").ToList();
            var quotedValues = _processor.ParseListValue("\"item1\", 'item2'").ToList();
            var arrayFormat = _processor.ParseListValue("[item1, item2, item3]").ToList();
            var empty = _processor.ParseListValue("").ToList();

            // Assert
            commaSeparated.Count.ShouldBe(3);
            commaSeparated.ShouldContain("item1");
            commaSeparated.ShouldContain("item2");
            commaSeparated.ShouldContain("item3");

            semiColonSeparated.Count.ShouldBe(3);
            semiColonSeparated.ShouldContain("item1");
            semiColonSeparated.ShouldContain("item2");
            semiColonSeparated.ShouldContain("item3");

            quotedValues.Count.ShouldBe(2);
            quotedValues.ShouldContain("item1");
            quotedValues.ShouldContain("item2");

            arrayFormat.Count.ShouldBe(3);
            arrayFormat.ShouldContain("item1");
            arrayFormat.ShouldContain("item2");
            arrayFormat.ShouldContain("item3");

            empty.Count.ShouldBe(0);
        }
    }
}