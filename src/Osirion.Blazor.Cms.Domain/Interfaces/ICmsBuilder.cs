using Microsoft.Extensions.DependencyInjection;

namespace Osirion.Blazor.Cms.Domain.Interfaces;

/// <summary>
/// Base builder interface for all CMS builder types
/// </summary>
public interface ICmsBuilder
{
    /// <summary>
    /// Gets the service collection being configured
    /// </summary>
    IServiceCollection Services { get; }
}