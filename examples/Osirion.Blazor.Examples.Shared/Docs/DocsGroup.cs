namespace Osirion.Blazor.Examples.Shared.Docs;

/// <summary>How a group appears in the documentation sidebar.</summary>
public enum DocsGroupKind
{
    /// <summary>Its pages sit at the top of the sidebar, without a heading.</summary>
    Start,

    /// <summary>A heading, linked to the group's page, with the group's pages under it.</summary>
    Section,
}

/// <summary>A link from a group to where more of its components are documented.</summary>
/// <param name="Text">What the link leads to.</param>
/// <param name="Url">The address.</param>
public sealed record DocsLink(string Text, string Url);

/// <summary>
/// A group of documentation pages. A page joins a group with <c>group: &lt;key&gt;</c> under <c>custom_fields</c> in its
/// front matter, and is placed in the group by its <c>order</c>, then its title.
/// </summary>
/// <param name="Key">The value pages give in their front matter.</param>
/// <param name="Title">The heading in the sidebar and in the component gallery.</param>
/// <param name="Kind">How the group appears in the sidebar.</param>
/// <param name="PageUrl">The group's own page, which its heading links to, or <see langword="null"/> when it has none.</param>
/// <param name="InGallery">Whether the component gallery shows the group.</param>
/// <param name="SeeAlso">Where more of the group's components are documented, when that is outside the group.</param>
public sealed record DocsGroup(
    string Key,
    string Title,
    DocsGroupKind Kind,
    string? PageUrl = null,
    bool InGallery = false,
    DocsLink? SeeAlso = null);
