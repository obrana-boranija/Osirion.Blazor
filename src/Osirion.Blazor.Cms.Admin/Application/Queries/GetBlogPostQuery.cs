using Osirion.Blazor.Cms.Application.Queries;
using Osirion.Blazor.Cms.Domain.Models;

namespace Osirion.Blazor.Cms.Admin.Application.Queries;

public class GetBlogPostQuery : IQuery<BlogPost>
{
    public string Path { get; set; } = string.Empty;
}