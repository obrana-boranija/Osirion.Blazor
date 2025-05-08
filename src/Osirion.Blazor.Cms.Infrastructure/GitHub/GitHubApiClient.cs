using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Octokit;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Options.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Osirion.Blazor.Cms.Infrastructure.GitHub;

/// <summary>
/// Client for GitHub API operations
/// </summary>
public class GitHubApiClient : IGitHubApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GitHubApiClient> _logger;
    private readonly IAuthenticationService _authService;
    private readonly JsonSerializerOptions _jsonOptions;

    private string? _owner;
    private string? _repository;
    private string _branch = "main";

    /// <summary>
    /// Initializes a new instance of the GitHubApiClient
    /// </summary>
    public GitHubApiClient(
        HttpClient httpClient,
        IOptions<GitHubAdminOptions> options,
        IAuthenticationService authService,
        ILogger<GitHubApiClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        var githubOptions = options?.Value ?? new GitHubAdminOptions();

        // Initialize options from config
        _owner = githubOptions.Owner;
        _repository = githubOptions.Repository;
        _branch = githubOptions.DefaultBranch;

        // Configure HTTP client
        _httpClient.BaseAddress = new Uri(githubOptions.ApiUrl ?? "https://api.github.com");
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
        _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("OsirionBlogCMS", "2.0"));

        // Setup token from authentication service if available
        if (!string.IsNullOrEmpty(_authService.AccessToken))
        {
            SetAccessToken(_authService.AccessToken);
        }

        // Subscribe to authentication changes
        _authService.AuthenticationChanged += OnAuthenticationChanged;

        // Configure JSON options
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    /// <summary>
    /// Sets the access token for API requests
    /// </summary>
    public void SetAccessToken(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            // Remove Authorization header if token is empty
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            return;
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", token);
    }

    /// <summary>
    /// Sets the repository for API requests
    /// </summary>
    public void SetRepository(string owner, string repository)
    {
        _owner = owner;
        _repository = repository;
    }

    /// <summary>
    /// Sets the branch for API requests
    /// </summary>
    public void SetBranch(string branch)
    {
        _branch = branch;
    }

    /// <summary>
    /// Gets the contents of a repository at a specific path
    /// </summary>
    public async Task<List<GitHubItem>> GetRepositoryContentsAsync(string path = "", CancellationToken cancellationToken = default)
    {
        ValidateRepositorySettings();

        var requestUrl = $"/repos/{_owner}/{_repository}/contents/{path}";
        if (!string.IsNullOrEmpty(_branch))
        {
            requestUrl += $"?ref={_branch}";
        }

        var response = await _httpClient.GetAsync(requestUrl, cancellationToken);
        response.EnsureSuccessStatusCode();

        // If response is an array, it's a directory
        if (response.Content.Headers.ContentType?.MediaType == "application/json" &&
            await IsJsonArrayAsync(response))
        {
            return await response.Content.ReadFromJsonAsync<List<GitHubItem>>(_jsonOptions, cancellationToken) ?? new List<GitHubItem>();
        }

        // If response is an object, it's a file
        var item = await response.Content.ReadFromJsonAsync<GitHubItem>(_jsonOptions, cancellationToken);
        return item != null ? new List<GitHubItem> { item } : new List<GitHubItem>();
    }

    /// <summary>
    /// Gets the content of a file
    /// </summary>
    public async Task<GitHubFileContent> GetFileContentAsync(string path, CancellationToken cancellationToken = default)
    {
        ValidateRepositorySettings();

        var requestUrl = $"/repos/{_owner}/{_repository}/contents/{path}";
        if (!string.IsNullOrEmpty(_branch))
        {
            requestUrl += $"?ref={_branch}";
        }

        var response = await _httpClient.GetAsync(requestUrl, cancellationToken);
        response.EnsureSuccessStatusCode();

        var fileContent = await response.Content.ReadFromJsonAsync<GitHubFileContent>(_jsonOptions, cancellationToken);
        return fileContent ?? throw new InvalidOperationException($"Failed to read file content for {path}");
    }

    /// <summary>
    /// Gets information about a file from the commit history
    /// </summary>
    public async Task<(DateTime Created, DateTime? Modified)> GetFileHistoryAsync(string path, CancellationToken cancellationToken = default)
    {
        ValidateRepositorySettings();

        // Get commits that modified this file
        var requestUrl = $"/repos/{_owner}/{_repository}/commits?path={path}";
        if (!string.IsNullOrEmpty(_branch))
        {
            requestUrl += $"&sha={_branch}";
        }

        var response = await _httpClient.GetAsync(requestUrl, cancellationToken);
        response.EnsureSuccessStatusCode();

        var commits = await response.Content.ReadFromJsonAsync<List<GitHubCommitInfo>>(_jsonOptions, cancellationToken)
            ?? new List<GitHubCommitInfo>();

        // Sort commits by date (oldest first)
        commits = commits.OrderBy(c => c.Commit?.Author?.Date).ToList();

        // First commit is the creation date
        var created = commits.FirstOrDefault()?.Commit?.Author?.Date ?? DateTime.UtcNow;

        // Last commit (if different from first) is the modified date
        DateTime? modified = null;
        if (commits.Count > 1)
        {
            modified = commits.LastOrDefault()?.Commit?.Author?.Date;
        }

        return (created, modified);
    }

    /// <summary>
    /// Creates or updates a file in the repository
    /// </summary>
    public async Task<GitHubFileCommitResponse> CreateOrUpdateFileAsync(
        string path, string content, string commitMessage, string? existingSha = null, CancellationToken cancellationToken = default)
    {
        ValidateRepositorySettings();

        var requestUrl = $"/repos/{_owner}/{_repository}/contents/{path}";

        var request = new GitHubFileUpdateRequest
        {
            Message = commitMessage,
            Content = Convert.ToBase64String(Encoding.UTF8.GetBytes(content)),
            Branch = _branch
        };

        // If sha is provided, it's an update
        if (!string.IsNullOrEmpty(existingSha))
        {
            request.Sha = existingSha;
        }

        var jsonContent = JsonSerializer.Serialize(request, _jsonOptions);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync(requestUrl, httpContent, cancellationToken);
        response.EnsureSuccessStatusCode();

        var commitResponse = await response.Content.ReadFromJsonAsync<GitHubFileCommitResponse>(_jsonOptions, cancellationToken);
        return commitResponse ?? throw new InvalidOperationException($"Failed to commit file {path}");
    }

    /// <summary>
    /// Deletes a file from the repository
    /// </summary>
    public async Task<GitHubFileCommitResponse> DeleteFileAsync(
        string path, string commitMessage, string sha, CancellationToken cancellationToken = default)
    {
        ValidateRepositorySettings();

        var requestUrl = $"/repos/{_owner}/{_repository}/contents/{path}";

        var request = new GitHubFileDeleteRequest
        {
            Message = commitMessage,
            Sha = sha,
            Branch = _branch
        };

        var jsonContent = JsonSerializer.Serialize(request, _jsonOptions);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var requestMessage = new HttpRequestMessage(HttpMethod.Delete, requestUrl)
        {
            Content = httpContent
        };

        var response = await _httpClient.SendAsync(requestMessage, cancellationToken);
        response.EnsureSuccessStatusCode();

        var commitResponse = await response.Content.ReadFromJsonAsync<GitHubFileCommitResponse>(_jsonOptions, cancellationToken);
        return commitResponse ?? throw new InvalidOperationException($"Failed to delete file {path}");
    }

    /// <summary>
    /// Gets the branches of a repository
    /// </summary>
    public async Task<List<GitHubBranch>> GetBranchesAsync(CancellationToken cancellationToken = default)
    {
        ValidateRepositorySettings();

        var requestUrl = $"/repos/{_owner}/{_repository}/branches";

        var response = await _httpClient.GetAsync(requestUrl, cancellationToken);
        response.EnsureSuccessStatusCode();

        var branches = await response.Content.ReadFromJsonAsync<List<GitHubBranch>>(_jsonOptions, cancellationToken);
        return branches ?? new List<GitHubBranch>();
    }

    /// <summary>
    /// Creates a new branch in the repository
    /// </summary>
    public async Task<GitHubBranch> CreateBranchAsync(string branchName, string fromBranch, CancellationToken cancellationToken = default)
    {
        ValidateRepositorySettings();

        // First, get the SHA of the commit to branch from
        var requestUrl = $"/repos/{_owner}/{_repository}/git/refs/heads/{fromBranch}";

        var response = await _httpClient.GetAsync(requestUrl, cancellationToken);
        response.EnsureSuccessStatusCode();

        var reference = await response.Content.ReadFromJsonAsync<GitHubReference>(_jsonOptions, cancellationToken);
        if (reference == null || reference.Object == null)
        {
            throw new InvalidOperationException($"Failed to get reference for branch {fromBranch}");
        }

        // Create the new branch
        var createRefUrl = $"/repos/{_owner}/{_repository}/git/refs";

        var request = new GitHubCreateReferenceRequest
        {
            Ref = $"refs/heads/{branchName}",
            Sha = reference.Object.Sha
        };

        var jsonContent = JsonSerializer.Serialize(request, _jsonOptions);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var createResponse = await _httpClient.PostAsync(createRefUrl, httpContent, cancellationToken);
        createResponse.EnsureSuccessStatusCode();

        // Return the branch information
        return new GitHubBranch
        {
            Name = branchName,
            Protected = false,
            Commit = new()
        };
    }

    /// <summary>
    /// Creates a pull request in the repository
    /// </summary>
    public async Task<GitHubPullRequest> CreatePullRequestAsync(
        string title, string body, string head, string baseBranch, CancellationToken cancellationToken = default)
    {
        ValidateRepositorySettings();

        var requestUrl = $"/repos/{_owner}/{_repository}/pulls";

        var request = new GitHubCreatePullRequest
        {
            Title = title,
            Body = body,
            Head = head,
            Base = baseBranch
        };

        var jsonContent = JsonSerializer.Serialize(request, _jsonOptions);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(requestUrl, httpContent, cancellationToken);
        response.EnsureSuccessStatusCode();

        var pullRequest = await response.Content.ReadFromJsonAsync<GitHubPullRequest>(_jsonOptions, cancellationToken);
        return pullRequest ?? throw new InvalidOperationException("Failed to create pull request");
    }

    /// <summary>
    /// Searches for files in the repository
    /// </summary>
    public async Task<GitHubSearchResult> SearchFilesAsync(string query, CancellationToken cancellationToken = default)
    {
        ValidateRepositorySettings();

        // Format repository qualifier
        var repoQualifier = $"repo:{_owner}/{_repository}";

        // Add branch qualifier if available
        var branchQualifier = !string.IsNullOrEmpty(_branch) ? $"+ref:{_branch}" : "";

        // Combine qualifiers with query
        var searchQuery = $"{query}+{repoQualifier}{branchQualifier}";

        // URL encode the query
        var encodedQuery = Uri.EscapeDataString(searchQuery);

        var requestUrl = $"/search/code?q={encodedQuery}";

        var response = await _httpClient.GetAsync(requestUrl, cancellationToken);
        response.EnsureSuccessStatusCode();

        var searchResult = await response.Content.ReadFromJsonAsync<GitHubSearchResult>(_jsonOptions, cancellationToken);
        return searchResult ?? new GitHubSearchResult
        {
            TotalCount = 0,
            IncompleteResults = false,
            Items = new List<GitHubItem>()
        };
    }

    /// <summary>
    /// Checks if the response contains a JSON array
    /// </summary>
    private async Task<bool> IsJsonArrayAsync(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        content = content.TrimStart();
        return content.StartsWith("[");
    }

    /// <summary>
    /// Validates repository settings
    /// </summary>
    private void ValidateRepositorySettings()
    {
        if (string.IsNullOrEmpty(_owner) || string.IsNullOrEmpty(_repository))
        {
            throw new InvalidOperationException("Repository owner and name must be set before making API requests");
        }
    }

    /// <summary>
    /// Handles authentication changes
    /// </summary>
    private void OnAuthenticationChanged(bool isAuthenticated)
    {
        if (isAuthenticated)
        {
            // Set token when authenticated
            SetAccessToken(_authService.AccessToken);
        }
        else
        {
            // Remove token when not authenticated
            SetAccessToken(null);
        }
    }
}

/// <summary>
/// Request model for updating a file
/// </summary>
internal class GitHubFileUpdateRequest
{
    public string Message { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Sha { get; set; }
    public string Branch { get; set; } = string.Empty;
}

/// <summary>
/// Request model for deleting a file
/// </summary>
internal class GitHubFileDeleteRequest
{
    public string Message { get; set; } = string.Empty;
    public string Sha { get; set; } = string.Empty;
    public string Branch { get; set; } = string.Empty;
}

/// <summary>
/// Response model for a reference
/// </summary>
internal class GitHubReference
{
    public string Ref { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public GitHubReferenceObject? Object { get; set; }
}

/// <summary>
/// Reference object model
/// </summary>
internal class GitHubReferenceObject
{
    public string Type { get; set; } = string.Empty;
    public string Sha { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}

/// <summary>
/// Request model for creating a reference
/// </summary>
internal class GitHubCreateReferenceRequest
{
    public string Ref { get; set; } = string.Empty;
    public string Sha { get; set; } = string.Empty;
}

/// <summary>
/// Request model for creating a pull request
/// </summary>
internal class GitHubCreatePullRequest
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Head { get; set; } = string.Empty;
    public string Base { get; set; } = string.Empty;
}

/// <summary>
/// Commit information model
/// </summary>
internal class GitHubCommitInfo
{
    public string Sha { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public GitHubCommitDetails? Commit { get; set; }
}

/// <summary>
/// Commit details model
/// </summary>
internal class GitHubCommitDetails
{
    public GitHubAuthor? Author { get; set; }
    public GitHubAuthor? Committer { get; set; }
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Author/committer information
/// </summary>
internal class GitHubAuthor
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}