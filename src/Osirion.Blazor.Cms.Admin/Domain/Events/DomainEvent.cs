namespace Osirion.Blazor.Cms.Admin.Domain.Events;

public abstract record DomainEvent(DateTime Timestamp)
{
    public DateTime Timestamp { get; } = Timestamp;
    public Guid Id { get; } = Guid.NewGuid();
}