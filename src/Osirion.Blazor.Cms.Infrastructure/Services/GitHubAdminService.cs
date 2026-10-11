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
/// <remarks>
/// Register this service as scoped. Each instance owns its GitHub API client, so the repository, branch and
/// token chosen in one circuit or request never reach another user or the public site. The client never
/// uses a content provider's configured API token: it uses the signed-in user's token, or the admin personal
/// access token configured in <see cref="AuthenticationOptions.PersonalAccessToken"/> when no user token is
/// set. Without either, repository operations send no request.
/// </remarks>
public class GitHubAdminService : IGitHubAdminService, IDisposable
{
    private readonly IGitHubApiClientFactory _apiClientFactory;
    private readonly IMarkdownProcessor _markdownProcessor;
    private readonly ILogger<GitHubAdminService> _logger;
    private readonly GitHubOptions _options;
    private readonly IAuthenticationService? _authenticationService;
    private readonly string? _configuredAccessToken;
    private IGitHubApiClient _apiClient;
    private string? _userAccessToken;
    private string _currentProviderName = string.Empty;

    /// <summary>Performs the GitHubAdminService operation.</summary>
    public GitHubAdminService(
        IGitHubApiClientFactory apiClientFactory,
        IOptions<CmsAdminOptions> options,
        IMarkdownProcessor markdownProcessor,
        ILogger<GitHubAdminService> logger)
        : this(apiClientFactory, options, markdownProcessor, logger, authenticationService: null)
    {
    }

    /// <summary>
    /// Initializes a new instance that follows the sign-in state of an authentication service
    /// </summary>
    /// <param name="apiClientFactory">The factory that creates this instance's GitHub API client</param>
    /// <param name="options">The CMS admin options</param>
    /// <param name="markdownProcessor">The markdown processor</param>
    /// <param name="logger">The logger instance</param>
    /// <param name="authenticationService">
    /// The authentication service of the same scope, or <see langword="null"/>. While its user is signed in,
    /// that user's token is used; on sign-out the token is dropped from this instance.
    /// </param>
    public GitHubAdminService(
        IGitHubApiClientFactory apiClientFactory,
        IOptions<CmsAdminOptions> options,
        IMarkdownProcessor markdownProcessor,
        ILogger<GitHubAdminService> logger,
        IAuthenticationService? authenticationService)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
        _markdownProcessor = markdownProcessor;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options.Value.GitHub;
        _configuredAccessToken = NullIfWhiteSpace(options.Value.Authentication?.PersonalAccessToken);

        _authenticationService = authenticationService;
        if (_authenticationService?.IsAuthenticated == true)
        {
            _userAccessToken = NullIfWhiteSpace(_authenticationService.AccessToken);
        }

        // Get default client
        _apiClient = CreateClient(providerName: null);

        // Subscribe last, so a constructor that throws leaves no handler behind
        if (_authenticationService is not null)
        {
            _authenticationService.AuthenticationChanged += OnAuthenticationChanged;
        }
    }

    /// <inheritdoc />
    public string CurrentBranch { get; private set; } = "main";
    /// <inheritdoc />
    public string CurrentRepository { get; private set; } = string.Empty;
    /// <inheritdoc />
    public string CurrentProvider { get; private set; } = string.Empty;

    /// <summary>
    /// Sets the current provider to use
    /// </summary>
    public void SetProvider(string providerName)
    {
        if (string.IsNullOrWhiteSpace(providerName))
        {
            _apiClient = CreateClient(providerName: null);
            CurrentProvider = "default";
        }
        else
        {
            _apiClient = CreateClient(providerName);
            CurrentProvider = providerName;
        }
        _currentProviderName = providerName;
        _logger.LogInformation("Switched to provider: {Provider}", CurrentProvider);
    }

    /// <inheritdoc />
    public async Task<GitHubBranch> CreateBranchAsync(string branchName, string fromBranch)
    {
        try
        {
            return await _apiClient.CreateBranchAsync(branchName, fromBranch)
                ?? throw new InvalidOperationException("GitHub did not return the created branch.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating branch {BranchName} from {FromBranch}", branchName, fromBranch);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<GitHubFileCommitResponse> CreateOrUpdateFileAsync(string path, string content, string commitMessage, string? existingSha = null)
    {
        try
        {
            return await _apiClient.CreateOrUpdateFileAsync(path, content, commitMessage, existingSha)
                ?? throw new InvalidOperationException("GitHub did not return the file commit response.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating/updating file {Path}", path);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<GitHubPullRequest> CreatePullRequestAsync(string title, string body, string head, string baseBranch)
    {
        try
        {
            return await _apiClient.CreatePullRequestAsync(title, body, head, baseBranch)
                ?? throw new InvalidOperationException("GitHub did not return the created pull request.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating pull request {Title}", title);
            throw;
        }
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
    public async Task<List<GitHubBranch>> GetBranchesAsync(string repository)
    {
        try
        {
            return await _apiClient.GetBranchesAsync() ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting branches for {Repository}", repository);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<GitHubFileContent> GetFileContentAsync(string path)
    {
        try
        {
            return await _apiClient.GetFileContentAsync(path)
                ?? throw new InvalidOperationException($"GitHub did not return file content for '{path}'.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file content {Path}", path);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<List<GitHubRepository>> GetRepositoriesAsync()
    {
        try
        {
            return await _apiClient.GetRepositoriesAsync() ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting repositories");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<List<GitHubItem>> GetRepositoryContentsAsync(string path = "")
    {
        try
        {
            return await _apiClient.GetRepositoryContentsAsync(path) ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting repository contents {Path}", path);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<List<GitHubItem>> SearchFilesAsync(string query)
    {
        try
        {
            var result = await _apiClient.SearchFilesAsync(query);
            return result?.Items ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching files with query {Query}", query);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task SetAuthTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            _logger.LogWarning("Attempted to set empty auth token");
            return;
        }

        _logger.LogInformation("Setting auth token in GitHubAdminService. Token length: {length}", token.Length);
        _userAccessToken = token;
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

    /// <inheritdoc />
    public void SetBranch(string branch)
    {
        CurrentBranch = branch;
        _apiClient.SetBranch(branch);
    }

    /// <inheritdoc />
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
    /// Stops following the sign-in state of the authentication service
    /// </summary>
    public void Dispose()
    {
        if (_authenticationService is not null)
        {
            _authenticationService.AuthenticationChanged -= OnAuthenticationChanged;
        }
    }

    /// <summary>
    /// The token this instance sends: the signed-in user's token, else the configured admin token, else none.
    /// A content provider's API token is never used.
    /// </summary>
    private string EffectiveAccessToken => _userAccessToken ?? _configuredAccessToken ?? string.Empty;

    /// <summary>
    /// Creates this instance's own client and replaces the provider's API token the factory seeded it with
    /// </summary>
    private IGitHubApiClient CreateClient(string? providerName)
    {
        var client = string.IsNullOrWhiteSpace(providerName)
            ? _apiClientFactory.GetDefaultClient()
            : _apiClientFactory.GetClient(providerName);

        client.SetAccessToken(EffectiveAccessToken);
        return client;
    }

    private void OnAuthenticationChanged(bool isAuthenticated)
    {
        // Sign-in adopts the user's token; sign-out drops it, leaving only the configured admin token
        _userAccessToken = isAuthenticated
            ? NullIfWhiteSpace(_authenticationService?.AccessToken) ?? _userAccessToken
            : null;

        _apiClient.SetAccessToken(EffectiveAccessToken);
    }

    private static string? NullIfWhiteSpace(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value;
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
