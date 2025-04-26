namespace Osirion.Blazor.Cms.Core.Providers.GitHub.Models;

public class GitHubPullRequest
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Head { get; set; } = string.Empty;
    public string Base { get; set; } = string.Empty;
}