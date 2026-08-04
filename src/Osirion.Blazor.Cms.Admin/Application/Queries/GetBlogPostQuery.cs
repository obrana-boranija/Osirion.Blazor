using Osirion.Blazor.Cms.Application.Queries;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Models;

namespace Osirion.Blazor.Cms.Admin.Application.Queries;

    /// <summary>Defines the public member API contract.</summary>
public class GetBlogPostQuery : IQuery<ContentItem>
{
    /// <summary>Gets or sets the Path value.</summary>
    public string Path { get; set; } = string.Empty;
}
