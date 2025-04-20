namespace Osirion.Blazor.Services.GitHub;

public class GitHubCommitAuthor
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}