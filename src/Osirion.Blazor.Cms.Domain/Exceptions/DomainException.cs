namespace Osirion.Blazor.Cms.Domain.Exceptions;

/// <summary>
/// Base exception for all domain exceptions
/// </summary>
public abstract class DomainException : Exception
{
    /// <summary>Gets or sets the DomainException value.</summary>
    protected DomainException(string message) : base(message) { }
    /// <summary>Gets or sets the DomainException value.</summary>
    protected DomainException(string message, Exception innerException)
        : base(message, innerException) { }
}
