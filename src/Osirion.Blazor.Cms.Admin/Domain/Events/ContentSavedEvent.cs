namespace Osirion.Blazor.Cms.Admin.Domain.Events;

public record ContentSavedDomainEvent(string Path, string Sha) : DomainEvent(DateTime.UtcNow);