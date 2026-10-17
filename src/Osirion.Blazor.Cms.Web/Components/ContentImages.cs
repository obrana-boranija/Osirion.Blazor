using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Web.Components;

/// <summary>
/// Helpers for the images of rendered content.
/// </summary>
internal static class ContentImages
{
    /// <summary>The custom front matter field that describes the featured image: <c>custom_fields: featured_image_alt</c>.</summary>
    internal const string FeaturedImageAltField = "featured_image_alt";

    /// <summary>
    /// The alternative text of an item's featured image: the <c>featured_image_alt</c> custom field of its front
    /// matter, otherwise an empty string. The front matter has no property for it, and the item's title would repeat
    /// the heading beside the image, so an image without a description is marked decorative (<c>alt=""</c>).
    /// </summary>
    /// <param name="item">The item whose featured image is shown.</param>
    /// <returns>The description from the front matter, or an empty string.</returns>
    internal static string FeaturedImageAlt(ContentItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return item.Metadata?.CustomFields is { } fields
            && fields.TryGetValue(FeaturedImageAltField, out var value)
            && value?.ToString() is { } text
            && !string.IsNullOrWhiteSpace(text)
                ? text.Trim()
                : string.Empty;
    }
}
