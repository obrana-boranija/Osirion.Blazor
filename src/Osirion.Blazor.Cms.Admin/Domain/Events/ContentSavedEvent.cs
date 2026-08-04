namespace Osirion.Blazor.Cms.Admin.Domain.Events;

/// <summary>Defines the ContentSavedDomainEvent API contract.</summary>
public record ContentSavedDomainEvent(string Path, string Sha) : DomainEvent(DateTime.UtcNow);
