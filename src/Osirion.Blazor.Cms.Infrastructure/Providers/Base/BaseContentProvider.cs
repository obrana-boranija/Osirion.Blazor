using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Infrastructure.Providers.Base
{
    /// <summary>
    /// Shared file‐loading and JSON‐deserialization logic.
    /// Keeps concrete providers simple (SRP) :contentReference[oaicite:2]{index=2}.
    /// </summary>
    public abstract class BaseContentProvider
    {
        protected readonly string RootFolder;

        protected BaseContentProvider(IConfiguration config)
        {
            RootFolder = config["Cms:RootFolder"]
                         ?? throw new ArgumentNullException("Cms:RootFolder");
        }

        protected async Task<ContentItem> LoadAsync(string path)
        {
            var json = await File.ReadAllTextAsync(path);
            return JsonSerializer.Deserialize<ContentItem>(json)
                   ?? throw new InvalidDataException($"Invalid JSON in {path}");
        }
    }
}
