namespace Osirion.Blazor.Cms.Domain.Interfaces;

public interface IDirectoryContentProvider
{
    Task<IEnumerable<string>> GetDirectoriesAsync(string path);
}
