using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Cms.Core.Components;

public partial class DocumentTree
{
    [Parameter] 
    public List<DocSection> Sections { get; set; } = new List<DocSection>();

    public class DocSection
    {
        public string Title { get; set; }
        public List<DocItem> Items { get; set; } = new List<DocItem>();
    }

    public class DocItem
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public bool IsActive { get; set; }
        public List<DocItem> Children { get; set; } = new List<DocItem>();
    }

    private string GetDocTreeClass()
    {
        return $"osirion-doc-tree-nav {CssClass}".Trim();
    }
}
