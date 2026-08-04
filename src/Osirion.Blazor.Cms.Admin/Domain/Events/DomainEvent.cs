namespace Osirion.Blazor.Cms.Admin.Domain.Events;

/// <summary>Defines the DomainEvent type.</summary>
public abstract record DomainEvent(DateTime Timestamp)
{
    /// <summary>Gets or sets the Timestamp value.</summary>
    public DateTime Timestamp { get; } = Timestamp;
    /// <summary>Gets or sets the Id value.</summary>
    public Guid Id { get; } = Guid.NewGuid();
}
