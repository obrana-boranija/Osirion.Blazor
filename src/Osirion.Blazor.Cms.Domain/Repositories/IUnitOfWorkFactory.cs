namespace Osirion.Blazor.Cms.Domain.Repositories;

/// <summary>
/// Factory interface for creating unit of work instances
/// </summary>
public interface IUnitOfWorkFactory
{
    /// <summary>
    /// Creates a unit of work for the specified provider
    /// </summary>
    /// <param name="providerId">Provider ID</param>
    /// <returns>Unit of work instance</returns>
    IUnitOfWork Create(string providerId);

    /// <summary>
    /// Creates a unit of work for the default provider
    /// </summary>
    /// <returns>Unit of work instance</returns>
    IUnitOfWork CreateForDefaultProvider();
}