namespace Osirion.Blazor.Core.Models;

/// <summary>
/// A link on <see cref="Osirion.Blazor.Components.OsirionAuthorCard"/>, such as the author's site or a social profile.
/// </summary>
/// <param name="Label">The link text, for example <c>GitHub</c>.</param>
/// <param name="Url">An http(s) or <c>mailto:</c> URL, or a path on the site; other schemes are not rendered.</param>
public sealed record AuthorLink(string Label, string Url);
