namespace Osirion.Blazor.Services.GitHub;

public class GitHubCommitDetail
{
    public GitHubCommitAuthor? Author { get; set; }
    public GitHubCommitAuthor? Committer { get; set; }
    public string Message { get; set; } = string.Empty;
}