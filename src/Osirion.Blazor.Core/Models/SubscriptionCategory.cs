namespace Osirion.Blazor.Core.Models;

/// <summary>
/// Represents a subscription category option.
/// </summary>
public class SubscriptionCategory
{
    /// <summary>
    /// Gets or sets the unique identifier for the category.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the category.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of what this category includes.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether this category is selected by default.
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// Gets or sets whether this category is required (cannot be unselected).
    /// </summary>
    public bool IsRequired { get; set; }
}