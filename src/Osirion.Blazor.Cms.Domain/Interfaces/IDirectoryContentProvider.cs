namespace Osirion.Blazor.Cms.Domain.Interfaces;

/// <summary>Defines the IDirectoryContentProvider API contract.</summary>
public interface IDirectoryContentProvider
{
    /// <summary>Performs the GetDirectories operation asynchronously.</summary>
    Task<IEnumerable<string>> GetDirectoriesAsync(string path);
}
