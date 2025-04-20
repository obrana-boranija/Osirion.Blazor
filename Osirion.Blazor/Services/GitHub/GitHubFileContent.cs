namespace Osirion.Blazor.Services.GitHub;

public class GitHubFileContent
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string Sha { get; set; } = string.Empty;
    public long Size { get; set; }
    public string Url { get; set; } = string.Empty;
    public string HtmlUrl { get; set; } = string.Empty;
    public string GitUrl { get; set; } = string.Empty;
    public string DownloadUrl { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Encoding { get; set; } = string.Empty;
}