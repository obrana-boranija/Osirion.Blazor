using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace Osirion.Blazor.Components;

/// <summary>Displays author and publication metadata for an article.</summary>
public partial class OsirionArticleMetdata
{
    /// <summary>Gets or sets the author's name, shown with a person icon. Hidden when empty or white space.</summary>
    [Parameter]
    public string? Author { get; set; }

    /// <summary>
    /// Gets or sets the publication date, shown with a calendar icon in the format of <see cref="DateFormat"/>.
    /// Hidden when <see langword="null"/>.
    /// </summary>
    [Parameter]
    public DateTime? PublishDate { get; set; }

    /// <summary>
    /// Gets or sets the .NET date format string for <see cref="PublishDate"/>, applied with the current culture.
    /// </summary>
    [Parameter]
    public string? DateFormat { get; set; } = "d";

    /// <summary>
    /// Gets or sets the read time in minutes, shown with a clock icon. The component appends " min", so pass the
    /// number only (<c>5</c>, not <c>5 min</c>). Hidden when empty or white space.
    /// </summary>
    [Parameter]
    public string? ReadTime { get; set; }

    private bool _hasMetadata => !string.IsNullOrWhiteSpace(Author) || PublishDate.HasValue || !string.IsNullOrWhiteSpace(ReadTime);
    /// <summary>
    /// Formats the publish date for display
    /// </summary>
    private string FormatPublishDate()
    {
        return PublishDate?.ToString(DateFormat, CultureInfo.CurrentCulture) ?? string.Empty;
    }
}
