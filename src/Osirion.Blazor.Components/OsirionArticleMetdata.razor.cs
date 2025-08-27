using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace Osirion.Blazor.Components;

public partial class OsirionArticleMetdata
{
    /// <summary>
    /// Gets or sets the author name
    /// </summary>
    [Parameter]
    public string? Author { get; set; }

    /// <summary>
    /// Gets or sets the publish date
    /// </summary>
    [Parameter]
    public DateTime? PublishDate { get; set; }

    /// <summary>
    /// Gets or sets the date formats
    /// </summary>
    [Parameter]
    public string? DateFormat { get; set; } = "d";

    /// <summary>
    /// Gets or sets the read time (for metadata)
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
