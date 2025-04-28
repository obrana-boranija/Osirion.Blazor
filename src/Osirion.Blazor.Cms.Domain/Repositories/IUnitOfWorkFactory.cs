// src/Osirion.Blazor.Cms.Domain/Repositories/IUnitOfWorkFactory.cs
namespace Osirion.Blazor.Cms.Domain.Repositories;

/// <summary>
/// Factory for creating unit of work instances
/// </summary>
public interface IUnitOfWorkFactory
{
    /// <summary>
    /// Creates a unit of work for the specified provider
    /// </summary>
    /// <param name="providerId">The provider ID</param>
    /// <returns>A unit of work for the provider</returns>
    IUnitOfWork Create(string providerId);

    /// <summary>
    /// Creates a unit of work for the default provider
    /// </summary>
    /// <returns>A unit of work for the default provider</returns>
    IUnitOfWork CreateForDefaultProvider();
}