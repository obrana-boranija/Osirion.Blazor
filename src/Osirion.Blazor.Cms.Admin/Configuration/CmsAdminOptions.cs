namespace Osirion.Blazor.Cms.Admin.Configuration;

public class CmsAdminOptions
{
    public string DefaultContentProvider { get; set; } = "GitHub";
    public bool EnableUserAuthentication { get; set; } = true;
    public bool PersistUserSelections { get; set; } = true;
    public int ContentCacheDurationMinutes { get; set; } = 10;
    public string BasePath { get; set; } = "/admin";
    public AuthenticationOptions Authentication { get; set; } = new();
    public GitHubOptions GitHub { get; set; } = new();
    public FileSystemOptions FileSystem { get; set; } = new();

    public class AuthenticationOptions
    {
        public bool EnableGitHubOAuth { get; set; } = false;
        public string GitHubClientId { get; set; } = string.Empty;
        public string GitHubClientSecret { get; set; } = string.Empty;
        public string GitHubRedirectUri { get; set; } = string.Empty;
        public bool AllowPersonalAccessTokens { get; set; } = true;
    }

    public class GitHubOptions
    {
        public string DefaultOwner { get; set; } = string.Empty;
        public string DefaultRepository { get; set; } = string.Empty;
        public string DefaultBranch { get; set; } = "main";
        public string ContentPath { get; set; } = string.Empty;
    }

    public class FileSystemOptions
    {
        public string RootPath { get; set; } = string.Empty;
        public string ContentPath { get; set; } = "content";
        public bool CreateDirectoriesIfNotExist { get; set; } = true;
    }
}