using Osirion.Blazor.Cms.Domain.ValueObjects;

namespace Osirion.Blazor.Cms.Tests.Domain.ValueObjects;

public class FrontMatterTests
{
    [Fact]
    public void Create_WithTitle_ReturnsValidInstance()
    {
        // Arrange
        string title = "Test Title";

        // Act
        var frontMatter = FrontMatter.Create(title);

        // Assert
        Assert.NotNull(frontMatter);
        Assert.Equal(title, frontMatter.Title);
        Assert.Empty(frontMatter.Description);
        Assert.Empty(frontMatter.Author);
        Assert.NotEmpty(frontMatter.Date); // Should set current date
        Assert.Null(frontMatter.FeaturedImage);
        Assert.Empty(frontMatter.Categories);
        Assert.Empty(frontMatter.Tags);
        Assert.False(frontMatter.IsFeatured);
        Assert.True(frontMatter.Published);
        Assert.Null(frontMatter.Layout);
        Assert.Null(frontMatter.Slug);
        Assert.Empty(frontMatter.CustomFields);
    }

    [Fact]
    public void Create_WithTitleAndDate_ReturnsValidInstance()
    {
        // Arrange
        string title = "Test Title";
        string description = "Test Description";
        DateTime date = new DateTime(2025, 1, 1);

        // Act
        var frontMatter = FrontMatter.Create(title, description, date);

        // Assert
        Assert.NotNull(frontMatter);
        Assert.Equal(title, frontMatter.Title);
        Assert.Equal(description, frontMatter.Description);
        Assert.Equal(date.ToString(), frontMatter.Date);
    }

    [Fact]
    public void Create_WithAllParameters_ReturnsValidInstance()
    {
        // Arrange
        string id = "post-1";
        string title = "Test Title";
        string description = "Test Description";
        string author = "Test Author";
        DateTime date = new DateTime(2025, 1, 1);
        string featuredImage = "image.jpg";
        var categories = new[] { "Category1", "Category2" };
        var tags = new[] { "Tag1", "Tag2" };
        bool isFeatured = true;
        bool published = false;
        string layout = "post";
        string slug = "custom-slug";
        var customFields = new Dictionary<string, object>
        {
            { "customField1", "value1" },
            { "customField2", 42 }
        };

        // Act
        var frontMatter = FrontMatter.Create(
            id,
            title,
            description,
            author,
            date,
            featuredImage,
            categories,
            tags,
            isFeatured,
            published,
            layout,
            slug,
            customFields);

        // Assert
        Assert.NotNull(frontMatter);
        Assert.Equal(id, frontMatter.Id);
        Assert.Equal(title, frontMatter.Title);
        Assert.Equal(description, frontMatter.Description);
        Assert.Equal(author, frontMatter.Author);
        Assert.Equal(date.ToString("yyyy-MM-dd"), frontMatter.Date);
        Assert.Equal(featuredImage, frontMatter.FeaturedImage);
        Assert.Equal(categories, frontMatter.Categories);
        Assert.Equal(tags, frontMatter.Tags);
        Assert.Equal(isFeatured, frontMatter.IsFeatured);
        Assert.Equal(published, frontMatter.Published);
        Assert.Equal(layout, frontMatter.Layout);
        Assert.Equal(slug, frontMatter.Slug);
        Assert.Equal(2, frontMatter.CustomFields.Count);
        Assert.Equal("value1", frontMatter.CustomFields["customField1"]);
        Assert.Equal(42, frontMatter.CustomFields["customField2"]);
    }

    [Fact]
    public void WithTitle_SetsTitle_ReturnsNewInstance()
    {
        // Arrange
        var original = FrontMatter.Create("Original Title");
        string newTitle = "New Title";

        // Act
        var modified = original.WithTitle(newTitle);

        // Assert
        Assert.NotSame(original, modified);
        Assert.Equal(newTitle, modified.Title);
        Assert.Equal("Original Title", original.Title);
    }

    [Fact]
    public void WithDescription_SetsDescription_ReturnsNewInstance()
    {
        // Arrange
        var original = FrontMatter.Create("Title");
        string description = "New Description";

        // Act
        var modified = original.WithDescription(description);

        // Assert
        Assert.NotSame(original, modified);
        Assert.Equal(description, modified.Description);
        Assert.Empty(original.Description);
    }

    [Fact]
    public void WithAuthor_SetsAuthor_ReturnsNewInstance()
    {
        // Arrange
        var original = FrontMatter.Create("Title");
        string author = "John Doe";

        // Act
        var modified = original.WithAuthor(author);

        // Assert
        Assert.NotSame(original, modified);
        Assert.Equal(author, modified.Author);
        Assert.Empty(original.Author);
    }

    [Fact]
    public void WithDate_SetsDate_ReturnsNewInstance()
    {
        // Arrange
        var original = FrontMatter.Create("Title");
        var date = new DateTime(2025, 1, 1);

        // Act
        var modified = original.WithDate(date);

        // Assert
        Assert.NotSame(original, modified);
        Assert.Equal(date.ToString("yyyy-MM-dd"), modified.Date);
    }

    [Fact]
    public void WithFeaturedImage_SetsImage_ReturnsNewInstance()
    {
        // Arrange
        var original = FrontMatter.Create("Title");
        string image = "image.jpg";

        // Act
        var modified = original.WithFeaturedImage(image);

        // Assert
        Assert.NotSame(original, modified);
        Assert.Equal(image, modified.FeaturedImage);
        Assert.Null(original.FeaturedImage);
    }

    [Fact]
    public void WithCategories_SetsCategories_ReturnsNewInstance()
    {
        // Arrange
        var original = FrontMatter.Create("Title");
        var categories = new[] { "Category1", "Category2" };

        // Act
        var modified = original.WithCategories(categories);

        // Assert
        Assert.NotSame(original, modified);
        Assert.Equal(categories, modified.Categories);
        Assert.Empty(original.Categories);
    }

    [Fact]
    public void WithTags_SetsTags_ReturnsNewInstance()
    {
        // Arrange
        var original = FrontMatter.Create("Title");
        var tags = new[] { "Tag1", "Tag2" };

        // Act
        var modified = original.WithTags(tags);

        // Assert
        Assert.NotSame(original, modified);
        Assert.Equal(tags, modified.Tags);
        Assert.Empty(original.Tags);
    }

    [Fact]
    public void WithFeatured_SetsFeatured_ReturnsNewInstance()
    {
        // Arrange
        var original = FrontMatter.Create("Title");

        // Act
        var modified = original.WithFeatured(true);

        // Assert
        Assert.NotSame(original, modified);
        Assert.True(modified.IsFeatured);
        Assert.False(original.IsFeatured);
    }

    [Fact]
    public void WithPublished_SetsPublished_ReturnsNewInstance()
    {
        // Arrange
        var original = FrontMatter.Create("Title");

        // Act
        var modified = original.WithPublished(false);

        // Assert
        Assert.NotSame(original, modified);
        Assert.False(modified.Published);
        Assert.True(original.Published);
    }

    [Fact]
    public void WithLayout_SetsLayout_ReturnsNewInstance()
    {
        // Arrange
        var original = FrontMatter.Create("Title");
        string layout = "post";

        // Act
        var modified = original.WithLayout(layout);

        // Assert
        Assert.NotSame(original, modified);
        Assert.Equal(layout, modified.Layout);
        Assert.Null(original.Layout);
    }

    [Fact]
    public void WithSlug_SetsSlug_ReturnsNewInstance()
    {
        // Arrange
        var original = FrontMatter.Create("Title");
        string slug = "custom-slug";

        // Act
        var modified = original.WithSlug(slug);

        // Assert
        Assert.NotSame(original, modified);
        Assert.Equal(slug, modified.Slug);
        Assert.Null(original.Slug);
    }

    [Fact]
    public void WithCustomFields_SetsCustomFields_ReturnsNewInstance()
    {
        // Arrange
        var original = FrontMatter.Create("Title");
        var customFields = new Dictionary<string, object>
        {
            { "customField1", "value1" },
            { "customField2", 42 }
        };

        // Act
        var modified = original.WithCustomFields(customFields);

        // Assert
        Assert.NotSame(original, modified);
        Assert.Equal(2, modified.CustomFields.Count);
        Assert.Equal("value1", modified.CustomFields["customField1"]);
        Assert.Equal(42, modified.CustomFields["customField2"]);
        Assert.Empty(original.CustomFields);
    }

    [Fact]
    public void WithCustomField_AddsOrUpdatesField_ReturnsNewInstance()
    {
        // Arrange
        var original = FrontMatter.Create("Title");

        // Act
        var modified1 = original.WithCustomField("field1", "value1");
        var modified2 = modified1.WithCustomField("field2", 42);
        var modified3 = modified2.WithCustomField("field1", "updated");

        // Assert
        Assert.NotSame(original, modified1);
        Assert.NotSame(modified1, modified2);
        Assert.NotSame(modified2, modified3);

        Assert.Empty(original.CustomFields);

        Assert.Single(modified1.CustomFields);
        Assert.Equal("value1", modified1.CustomFields["field1"]);

        Assert.Equal(2, modified2.CustomFields.Count);
        Assert.Equal("value1", modified2.CustomFields["field1"]);
        Assert.Equal(42, modified2.CustomFields["field2"]);

        Assert.Equal(2, modified3.CustomFields.Count);
        Assert.Equal("updated", modified3.CustomFields["field1"]);
        Assert.Equal(42, modified3.CustomFields["field2"]);
    }

    [Fact]
    public void WithCustomField_WithEmptyKey_ThrowsArgumentException()
    {
        // Arrange
        var frontMatter = FrontMatter.Create("Title");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => frontMatter.WithCustomField("", "value"));
    }

    [Fact]
    public void ToYaml_OutputsCorrectFormat()
    {
        // Arrange
        var frontMatter = FrontMatter.Create("Test Title")
            .WithDescription("Test Description")
            .WithAuthor("John Doe")
            .WithDate(new DateTime(2025, 1, 1))
            .WithFeaturedImage("image.jpg")
            .WithCategories(new[] { "Category1", "Category2" })
            .WithTags(new[] { "Tag1", "Tag2" })
            .WithFeatured(true)
            .WithPublished(true)
            .WithLayout("post")
            .WithSlug("test-title")
            .WithCustomField("customField", "customValue");

        // Act
        string yaml = frontMatter.ToYaml();

        // Assert
        Assert.Contains("---", yaml);
        Assert.Contains("title: \"Test Title\"", yaml);
        Assert.Contains("description: \"Test Description\"", yaml);
        Assert.Contains("author: \"John Doe\"", yaml);
        Assert.Contains("date: 2025-01-01", yaml);
        Assert.Contains("featuredImage: \"image.jpg\"", yaml);
        Assert.Contains("categories:", yaml);
        Assert.Contains("  - \"Category1\"", yaml);
        Assert.Contains("  - \"Category2\"", yaml);
        Assert.Contains("tags:", yaml);
        Assert.Contains("  - \"Tag1\"", yaml);
        Assert.Contains("  - \"Tag2\"", yaml);
        Assert.Contains("featured: true", yaml);
        Assert.Contains("layout: \"post\"", yaml);
        Assert.Contains("slug: \"test-title\"", yaml);
        Assert.Contains("customField: \"customValue\"", yaml);
    }

    [Fact]
    public void Clone_CreatesDeepCopy()
    {
        // Arrange
        var original = FrontMatter.Create("Test Title")
            .WithDescription("Test Description")
            .WithAuthor("John Doe")
            .WithDate(new DateTime(2025, 1, 1))
            .WithFeaturedImage("image.jpg")
            .WithCategories(new[] { "Category1", "Category2" })
            .WithTags(new[] { "Tag1", "Tag2" })
            .WithFeatured(true)
            .WithPublished(false)
            .WithLayout("post")
            .WithSlug("test-title")
            .WithCustomField("customField", "customValue");

        // Act
        var clone = original.Clone();

        // Assert
        Assert.NotSame(original, clone);
        Assert.Equal(original.Title, clone.Title);
        Assert.Equal(original.Description, clone.Description);
        Assert.Equal(original.Author, clone.Author);
        Assert.Equal(original.Date, clone.Date);
        Assert.Equal(original.FeaturedImage, clone.FeaturedImage);

        Assert.Equal(original.Categories.Count, clone.Categories.Count);
        foreach (var category in original.Categories)
        {
            Assert.Contains(category, clone.Categories);
        }

        Assert.Equal(original.Tags.Count, clone.Tags.Count);
        foreach (var tag in original.Tags)
        {
            Assert.Contains(tag, clone.Tags);
        }

        Assert.Equal(original.IsFeatured, clone.IsFeatured);
        Assert.Equal(original.Published, clone.Published);
        Assert.Equal(original.Layout, clone.Layout);
        Assert.Equal(original.Slug, clone.Slug);

        Assert.Equal(original.CustomFields.Count, clone.CustomFields.Count);
        foreach (var key in original.CustomFields.Keys)
        {
            Assert.Equal(original.CustomFields[key], clone.CustomFields[key]);
        }
    }

    [Fact]
    public void FromDictionary_CreatesInstanceFromDictionary()
    {
        // Arrange
        var dictionary = new Dictionary<string, string>
        {
            { "title", "Dictionary Title" },
            { "description", "Dictionary Description" },
            { "author", "Dictionary Author" },
            { "date", "2025-02-15" },
            { "featuredImage", "dictionary-image.jpg" },
            { "categories", "Category1, Category2" },
            { "tags", "Tag1, Tag2" },
            { "featured", "true" },
            { "published", "false" },
            { "layout", "dictionary-layout" },
            { "slug", "dictionary-slug" },
            { "customField1", "customValue1" },
            { "customField2", "42" }
        };

        // Act
        var frontMatter = FrontMatter.FromDictionary(dictionary);

        // Assert
        Assert.Equal("Dictionary Title", frontMatter.Title);
        Assert.Equal("Dictionary Description", frontMatter.Description);
        Assert.Equal("Dictionary Author", frontMatter.Author);
        Assert.Equal("2025-02-15", frontMatter.Date);
        Assert.Equal("dictionary-image.jpg", frontMatter.FeaturedImage);
        Assert.Equal(2, frontMatter.Categories.Count);
        Assert.Contains("Category1", frontMatter.Categories);
        Assert.Contains("Category2", frontMatter.Categories);
        Assert.Equal(2, frontMatter.Tags.Count);
        Assert.Contains("Tag1", frontMatter.Tags);
        Assert.Contains("Tag2", frontMatter.Tags);
        Assert.True(frontMatter.IsFeatured);
        Assert.False(frontMatter.Published);
        Assert.Equal("dictionary-layout", frontMatter.Layout);
        Assert.Equal("dictionary-slug", frontMatter.Slug);
        Assert.Equal(2, frontMatter.CustomFields.Count);
        Assert.Equal("customValue1", frontMatter.CustomFields["customField1"]);
        Assert.Equal(42, frontMatter.CustomFields["customField2"]);
    }

    [Fact]
    public void FromDictionary_WithNullDictionary_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => FrontMatter.FromDictionary(null));
    }

    [Fact]
    public void FromMarkdown_ExtractsFrontMatter()
    {
        // Arrange
        var markdown = @"---
title: ""Markdown Title""
description: ""Markdown Description""
author: ""Markdown Author""
date: 2025-03-20
categories:
  - ""Category1""
  - ""Category2""
tags:
  - ""Tag1""
  - ""Tag2""
featured: true
slug: ""markdown-slug""
customField: ""customValue""
---

# This is the content

This is a paragraph.";

        // Act
        var frontMatter = FrontMatter.FromMarkdown(markdown);

        // Assert
        Assert.Equal("Markdown Title", frontMatter.Title);
        Assert.Equal("Markdown Description", frontMatter.Description);
        Assert.Equal("Markdown Author", frontMatter.Author);
        Assert.Equal("2025-03-20", frontMatter.Date);
        Assert.Contains("Category1", frontMatter.Categories);
        Assert.Contains("Category2", frontMatter.Categories);
        Assert.Contains("Tag1", frontMatter.Tags);
        Assert.Contains("Tag2", frontMatter.Tags);
        Assert.True(frontMatter.IsFeatured);
        Assert.Equal("markdown-slug", frontMatter.Slug);
        Assert.Equal("customValue", frontMatter.CustomFields["customField"]);
    }

    [Fact]
    public void FromMarkdown_WithoutFrontMatter_ReturnsEmptyFrontMatter()
    {
        // Arrange
        var markdown = @"# This is a simple markdown
Without any frontmatter.";

        // Act
        var frontMatter = FrontMatter.FromMarkdown(markdown);

        // Assert
        Assert.Empty(frontMatter.Title);
        Assert.Empty(frontMatter.Categories);
        Assert.Empty(frontMatter.Tags);
        Assert.Empty(frontMatter.CustomFields);
    }

    [Fact]
    public void FromYaml_ParsesYamlString()
    {
        // Arrange
        var yaml = @"---
title: YAML Title
description: YAML Description
author: YAML Author
date: 2025-04-25
featuredImage: yaml-image.jpg
categories:
  - YAML-Category1
  - YAML-Category2
tags:
  - YAML-Tag1
  - YAML-Tag2
featured: true
layout: yaml-layout
slug: yaml-slug
customYaml: customValue
---";

        // Act
        var frontMatter = FrontMatter.FromYaml(yaml);

        // Assert
        Assert.Equal("YAML Title", frontMatter.Title);
        Assert.Equal("YAML Description", frontMatter.Description);
        Assert.Equal("YAML Author", frontMatter.Author);
        Assert.Equal("2025-04-25", frontMatter.Date);
        Assert.Equal("yaml-image.jpg", frontMatter.FeaturedImage);
        Assert.Contains("YAML-Category1", frontMatter.Categories);
        Assert.Contains("YAML-Category2", frontMatter.Categories);
        Assert.Contains("YAML-Tag1", frontMatter.Tags);
        Assert.Contains("YAML-Tag2", frontMatter.Tags);
        Assert.True(frontMatter.IsFeatured);
        Assert.Equal("yaml-layout", frontMatter.Layout);
        Assert.Equal("yaml-slug", frontMatter.Slug);
        Assert.Equal("customValue", frontMatter.CustomFields["customYaml"]);
    }

    [Fact]
    public void Equals_WithSameValues_ReturnsTrue()
    {
        // Arrange
        var fm1 = FrontMatter.Create("Title")
            .WithDescription("Description")
            .WithTags(new[] { "Tag1", "Tag2" });

        var fm2 = FrontMatter.Create("Title")
            .WithDescription("Description")
            .WithTags(new[] { "Tag1", "Tag2" });

        // Act & Assert
        Assert.Equal(fm1, fm2);
    }

    [Fact]
    public void Equals_WithDifferentValues_ReturnsFalse()
    {
        // Arrange
        var fm1 = FrontMatter.Create("Title1");
        var fm2 = FrontMatter.Create("Title2");

        // Act & Assert
        Assert.NotEqual(fm1, fm2);
    }

    [Fact]
    public void GetHashCode_WithSameValues_ReturnsSameHashCode()
    {
        // Arrange
        var fm1 = FrontMatter.Create("Title")
            .WithDescription("Description")
            .WithTags(new[] { "Tag1", "Tag2" });

        var fm2 = FrontMatter.Create("Title")
            .WithDescription("Description")
            .WithTags(new[] { "Tag1", "Tag2" });

        // Act & Assert
        Assert.Equal(fm1.GetHashCode(), fm2.GetHashCode());
    }
}