namespace Osirion.Blazor.Services.GitHub;

public class GitHubCommit
{
    public string Sha { get; set; } = string.Empty;
    public GitHubCommitDetail? Commit { get; set; }
}