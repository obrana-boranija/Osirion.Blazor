using Markdig;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Options.Configuration;
using System.Text.RegularExpressions;

namespace Osirion.Blazor.Cms.Infrastructure.Services;

/// <summary>
/// Implementation of IGitHubAdminService for managing GitHub repositories
/// </summary>
public class GitHubAdminService : IGitHubAdminService
{
    private readonly IGitHubApiClientFactory _apiClientFactory;
    private readonly IMarkdownProcessor _markdownProcessor;
    private readonly ILogger<GitHubAdminService> _logger;
    private readonly GitHubOptions _options;
    private IGitHubApiClient _apiClient;
    private string _currentProviderName = string.Empty;

    public GitHubAdminService(
        IGitHubApiClientFactory apiClientFactory,
        IOptions<CmsAdminOptions> options,
        IMarkdownProcessor markdownProcessor,
        ILogger<GitHubAdminService> logger)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
        _markdownProcessor = markdownProcessor;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options.Value.GitHub;

        // Get default client
        _apiClient = _apiClientFactory.GetDefaultClient();
    }

    public string CurrentBranch { get; private set; } = "main";
    public string CurrentRepository { get; private set; } = string.Empty;
    public string CurrentProvider { get; private set; } = string.Empty;

    /// <summary>
    /// Sets the current provider to use
    /// </summary>
    public void SetProvider(string providerName)
    {
        if (string.IsNullOrWhiteSpace(providerName))
        {
            _apiClient = _apiClientFactory.GetDefaultClient();
            CurrentProvider = "default";
        }
        else
        {
            _apiClient = _apiClientFactory.GetClient(providerName);
            CurrentProvider = providerName;
        }
        _currentProviderName = providerName;
        _logger.LogInformation("Switched to provider: {Provider}", CurrentProvider);
    }

    public async Task<GitHubBranch> CreateBranchAsync(string branchName, string fromBranch)
    {
        try
        {
            return await _apiClient.CreateBranchAsync(branchName, fromBranch);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating branch {BranchName} from {FromBranch}", branchName, fromBranch);
            throw;
        }
    }

    public async Task<GitHubFileCommitResponse> CreateOrUpdateFileAsync(string path, string content, string commitMessage, string? existingSha = null)
    {
        try
        {
            return await _apiClient.CreateOrUpdateFileAsync(path, content, commitMessage, existingSha);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating/updating file {Path}", path);
            throw;
        }
    }

    public async Task<GitHubPullRequest> CreatePullRequestAsync(string title, string body, string head, string baseBranch)
    {
        try
        {
            return await _apiClient.CreatePullRequestAsync(title, body, head, baseBranch);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating pull request {Title}", title);
            throw;
        }
    }

    public async Task<GitHubFileCommitResponse> DeleteFileAsync(string path, string commitMessage, string sha)
    {
        try
        {
            return await _apiClient.DeleteFileAsync(path, commitMessage, sha);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file {Path}", path);
            throw;
        }
    }

    public async Task<ContentItem> GetBlogPostAsync(string path)
    {
        try
        {
            var fileContent = await _apiClient.GetFileContentAsync(path);
            return FromGitHubFile(fileContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting blog post {Path}", path);
            throw;
        }
    }

    public async Task<List<GitHubBranch>> GetBranchesAsync(string repository)
    {
        try
        {
            return await _apiClient.GetBranchesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting branches for {Repository}", repository);
            throw;
        }
    }

    public async Task<GitHubFileContent> GetFileContentAsync(string path)
    {
        try
        {
            return await _apiClient.GetFileContentAsync(path);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file content {Path}", path);
            throw;
        }
    }

    public async Task<List<GitHubRepository>> GetRepositoriesAsync()
    {
        try
        {
            return await _apiClient.GetRepositoriesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting repositories");
            throw;
        }
    }

    public async Task<List<GitHubItem>> GetRepositoryContentsAsync(string path = "")
    {
        try
        {
            return await _apiClient.GetRepositoryContentsAsync(path);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting repository contents {Path}", path);
            throw;
        }
    }

    public async Task<List<GitHubItem>> SearchFilesAsync(string query)
    {
        try
        {
            var result = await _apiClient.SearchFilesAsync(query);
            return result.Items;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching files with query {Query}", query);
            throw;
        }
    }

    public async Task SetAuthTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            _logger.LogWarning("Attempted to set empty auth token");
            return;
        }

        _logger.LogInformation("Setting auth token in GitHubAdminService. Token length: {length}", token.Length);
        _apiClient.SetAccessToken(token);

        // Test the token works
        try
        {
            await _apiClient.GetRepositoriesAsync();
            _logger.LogInformation("Token successfully verified");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token validation failed");
        }
    }

    public void SetBranch(string branch)
    {
        CurrentBranch = branch;
        _apiClient.SetBranch(branch);
    }

    public void SetRepository(string repository)
    {
        CurrentRepository = repository;

        // Parse owner/repo format
        var parts = repository.Split('/');
        if (parts.Length == 2)
        {
            _apiClient.SetRepository(parts[0], parts[1]);
        }
        else
        {
            _logger.LogWarning("Invalid repository format: {Repository}. Expected format: owner/repo", repository);
        }
    }

    /// <summary>
    /// Creates a blog post from a GitHub file content object
    /// </summary>
    /// <param name="fileContent">The GitHub file content</param>
    /// <returns>A blog post object</returns>
    private ContentItem FromGitHubFile(GitHubFileContent? fileContent)
    {
        var blogPost = new ContentItem();

        if (fileContent is null)
        {
            return blogPost;
        }

        // Set file information
        blogPost.Path = fileContent.Path;
        blogPost.Sha = fileContent.Sha;

        if (fileContent.IsMarkdownFile())
        {
            // If it's a markdown file, decode and parse the content
            var content = fileContent.GetDecodedContent();
            var extracted = _markdownProcessor.ExtractFrontMatterAndContent(content);
            blogPost.Metadata = extracted.FrontMatter;
            blogPost.Content = extracted.Content;
        }

        return blogPost;
    }

    /// <summary>
    /// Creates a blog post from markdown with frontmatter
    /// </summary>
    /// <param name="markdown">The full markdown content with frontmatter</param>
    /// <returns>A blog post object</returns>
    private ContentItem FromMarkdown(string markdown)
    {
        var blogPost = new ContentItem();

        if (string.IsNullOrWhiteSpace(markdown))
        {
            return blogPost;
        }

        var extracted = _markdownProcessor.ExtractFrontMatterAndContent(markdown);
        blogPost.Metadata = extracted.FrontMatter;
        blogPost.Content = extracted.Content;

        return blogPost;
    }
}