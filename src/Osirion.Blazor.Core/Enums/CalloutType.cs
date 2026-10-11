namespace Osirion.Blazor.Components;

/// <summary>
/// The kinds of callout, matching GitHub's Markdown alerts (<c>&gt; [!NOTE]</c> and the others) so a callout written
/// in Markdown and an <see cref="OsirionCallout"/> look the same.
/// </summary>
public enum CalloutType
{
    /// <summary>Useful information the reader should notice even when skimming.</summary>
    Note,

    /// <summary>Optional advice that helps the reader do something better or more easily.</summary>
    Tip,

    /// <summary>Key information the reader needs to succeed.</summary>
    Important,

    /// <summary>Urgent information that needs the reader's attention to avoid problems.</summary>
    Warning,

    /// <summary>Advice about the risks or negative outcomes of an action.</summary>
    Caution,
}
