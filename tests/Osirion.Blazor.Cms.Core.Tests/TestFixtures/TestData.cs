using Osirion.Blazor.Cms.Core.Components;

namespace Osirion.Blazor.Cms.Core.Tests.TestFixtures;

/// <summary>
/// Contains test data for component and service tests
/// </summary>
public static class TestData
{
    /// <summary>
    /// Sample basic markdown content
    /// </summary>
    public const string BasicMarkdown = "# Test Heading\n\nThis is a paragraph with **bold** and *italic* text.\n\n- List item 1\n- List item 2";

    /// <summary>
    /// Sample markdown content with frontmatter
    /// </summary>
    public const string MarkdownWithFrontmatter =
                            @"---
                            title: Test Page
                            description: This is a test page
                            date: 2025-05-01
                            author: Test Author
                            tags: [test, markdown]
                            ---

                            # Test Heading

                            This is a test paragraph.";

    /// <summary>
    /// Sample rendered HTML from markdown
    /// </summary>
    public const string RenderedHtml = "<h1>Test Heading</h1>\n<p>This is a paragraph with <strong>bold</strong> and <em>italic</em> text.</p>\n<ul>\n<li>List item 1</li>\n<li>List item 2</li>\n</ul>";

    /// <summary>
    /// Sample document tree data for navigation tests
    /// </summary>
    public static List<DocumentTree.DocSection> GetSampleDocTree()
    {
        return new List<DocumentTree.DocSection>
        {
            new DocumentTree.DocSection
            {
                Title = "Getting Started",
                Items = new List<DocumentTree.DocItem>
                {
                    new DocumentTree.DocItem { Title = "Introduction", Url = "/docs/intro", IsActive = true },
                    new DocumentTree.DocItem { Title = "Installation", Url = "/docs/installation", IsActive = false },
                    new DocumentTree.DocItem
                    {
                        Title = "Configuration",
                        Url = "/docs/config",
                        IsActive = false,
                        Children = new List<DocumentTree.DocItem>
                        {
                            new DocumentTree.DocItem { Title = "Basic Setup", Url = "/docs/config/basic", IsActive = false },
                            new DocumentTree.DocItem { Title = "Advanced Options", Url = "/docs/config/advanced", IsActive = false }
                        }
                    }
                }
            },
            new DocumentTree.DocSection
            {
                Title = "Components",
                Items = new List<DocumentTree.DocItem>
                {
                    new DocumentTree.DocItem { Title = "Markdown Editor", Url = "/docs/components/markdown", IsActive = false },
                    new DocumentTree.DocItem { Title = "Document Tree", Url = "/docs/components/doctree", IsActive = false }
                }
            }
        };
    }
}