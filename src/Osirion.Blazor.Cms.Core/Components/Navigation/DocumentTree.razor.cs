using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Cms.Core.Components;

/// <summary>
/// Represents a document tree component for navigation.
/// </summary>
public partial class DocumentTree
{
    /// <summary>
    /// Gets or sets the sections in the document tree.
    /// </summary>
    [Parameter]
    public List<DocSection> Sections { get; set; } = new List<DocSection>();


    /// <summary>
    /// Represents a section in the document tree.
    /// </summary>
    public class DocSection
    {
        /// <summary>
        /// Gets or sets the title of the section.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the items in the section.
        /// </summary>
        public List<DocItem> Items { get; set; } = new List<DocItem>();
    }


    /// <summary>
    /// Represents an item in the document tree.
    /// </summary>
    public class DocItem
    {
        /// <summary>
        /// Gets or sets the title of the item.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the URL of the item.
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the item is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the child items.
        /// </summary>
        public List<DocItem> Children { get; set; } = new List<DocItem>();
    }

    private string GetDocTreeClass()
    {
        return $"osirion-doc-tree-nav {Class}".Trim();
    }
}
