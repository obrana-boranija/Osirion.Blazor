using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Octokit;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.Options.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Osirion.Blazor.Cms.Infrastructure.GitHub;

/// <summary>
/// GitHub API client implementation
/// </summary>
public class GitHubApiClient : IGitHubApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GitHubApiClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    private string _owner = string.Empty;
    private string _repository = string.Empty;
    private string _branch = "main";
    private string _accessToken = string.Empty;
    private string _apiUrl = "https://api.github.com";

    public GitHubApiClient(
        HttpClient httpClient,
        IOptions<CmsAdminOptions> options,
        ILogger<GitHubApiClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        var githubOptions = options.Value.GitHub;
        _owner = githubOptions.Owner;
        _repository = githubOptions.Repository;
        _branch = githubOptions.DefaultBranch;
        _apiUrl = githubOptions.ApiUrl ?? string.Empty;

        var authOptions = options.Value.Authentication;
        _accessToken = authOptions.PersonalAccessToken ?? string.Empty;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        ConfigureHttpClient();
    }

    /// <summary>
    /// Gets a list of repositories the authenticated user has access to
    /// </summary>
    public async Task<List<GitHubRepository>> GetRepositoriesAsync(CancellationToken cancellationToken = default!)
    {
        try
        {
            // Check if we have an access token
            if (string.IsNullOrEmpty(_accessToken))
            {
                throw new UnauthorizedAccessException("Access token is required to list repositories");
            }

            // Prepare the request - get authenticated user's repos (or organization repos if _owner is an organization)
            string endpoint;

            // If _owner is set, try to get repositories for that organization/user
            if (!string.IsNullOrEmpty(_owner))
            {
                endpoint = $"/users/{_owner}/repos";

                // Check if the owner is an organization
                try
                {
                    var orgResponse = await _httpClient.GetAsync($"{_apiUrl}/orgs/{_owner}", cancellationToken);
                    if (orgResponse.IsSuccessStatusCode)
                    {
                        endpoint = $"/orgs/{_owner}/repos";
                    }
                }
                catch
                {
                    // If checking organization fails, default to user repos
                }
            }
            else
            {
                // Default to authenticated user's repos
                endpoint = "/user/repos";
            }

            // Make the API request
            var response = await _httpClient.GetAsync($"{_apiUrl}{endpoint}", cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var repositories = JsonSerializer.Deserialize<List<GitHubRepository>>(content, _jsonOptions);

            return repositories ?? new List<GitHubRepository>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching GitHub repositories");
            throw;
        }
    }

    /// <summary>
    /// Gets the branches for the current repository
    /// </summary>
    public async Task<List<GitHubBranch>> GetBranchesAsync(CancellationToken cancellationToken = default!)
    {
        try
        {
            ValidateRepositoryConfig();

            var endpoint = $"/repos/{_owner}/{_repository}/branches";
            var response = await _httpClient.GetAsync($"{_apiUrl}{endpoint}", cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var branches = JsonSerializer.Deserialize<List<GitHubBranch>>(content, _jsonOptions);

            return branches ?? new List<GitHubBranch>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching branches for repository {Owner}/{Repository}", _owner, _repository);
            throw;
        }
    }

    /// <summary>
    /// Gets the contents of the repository at the specified path
    /// </summary>
    public async Task<List<GitHubItem>> GetRepositoryContentsAsync(string path, CancellationToken cancellationToken = default!)
    {
        try
        {
            ValidateRepositoryConfig();

            var endpoint = $"/repos/{_owner}/{_repository}/contents/{path}";
            if (!string.IsNullOrEmpty(_branch))
            {
                endpoint += $"?ref={_branch}";
            }

            var response = await _httpClient.GetAsync($"{_apiUrl}{endpoint}", cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            // GitHub API can return either a single item or an array of items
            try
            {
                // Try to deserialize as array first
                var items = JsonSerializer.Deserialize<List<GitHubItem>>(content, _jsonOptions);
                return items ?? new List<GitHubItem>();
            }
            catch
            {
                // If that fails, try to deserialize as a single item
                var item = JsonSerializer.Deserialize<GitHubItem>(content, _jsonOptions);
                return item != null ? new List<GitHubItem> { item } : new List<GitHubItem>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching repository contents at path {Path}", path);
            throw;
        }
    }

    /// <summary>
    /// Gets the content of a file in the repository
    /// </summary>
    public async Task<GitHubFileContent> GetFileContentAsync(string path, CancellationToken cancellationToken = default!)
    {
        try
        {
            ValidateRepositoryConfig();

            var endpoint = $"/repos/{_owner}/{_repository}/contents/{path}";
            if (!string.IsNullOrEmpty(_branch))
            {
                endpoint += $"?ref={_branch}";
            }

            var response = await _httpClient.GetAsync($"{_apiUrl}{endpoint}", cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var fileContent = JsonSerializer.Deserialize<GitHubFileContent>(content, _jsonOptions);

            if (fileContent == null)
            {
                throw new Exception($"Failed to deserialize file content for {path}");
            }

            return fileContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching file content for {Path}", path);
            throw;
        }
    }

    /// <summary>
    /// Creates or updates a file in the repository
    /// </summary>
    public async Task<GitHubFileCommitResponse> CreateOrUpdateFileAsync(string path, string content, string message, string? sha = null, CancellationToken cancellationToken = default!)
    {
        try
        {
            ValidateRepositoryConfig();

            var endpoint = $"/repos/{_owner}/{_repository}/contents/{path}";

            // Convert content to base64
            var contentBytes = Encoding.UTF8.GetBytes(content);
            var base64Content = Convert.ToBase64String(contentBytes);

            // Create request body
            var requestBody = new
            {
                message,
                content = base64Content,
                branch = _branch,
                sha // Include SHA only for updates
            };

            var json = JsonSerializer.Serialize(requestBody);
            var requestContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_apiUrl}{endpoint}", requestContent, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var commitResponse = JsonSerializer.Deserialize<GitHubFileCommitResponse>(responseContent, _jsonOptions);

            if (commitResponse == null)
            {
                throw new Exception($"Failed to deserialize commit response for {path}");
            }

            return commitResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating/updating file {Path}", path);
            throw;
        }
    }

    /// <summary>
    /// Deletes a file from the repository
    /// </summary>
    public async Task<GitHubFileCommitResponse> DeleteFileAsync(string path, string message, string sha, CancellationToken cancellationToken = default!)
    {
        try
        {
            ValidateRepositoryConfig();

            var endpoint = $"/repos/{_owner}/{_repository}/contents/{path}";

            // Create request body
            var requestBody = new
            {
                message,
                sha,
                branch = _branch
            };

            var json = JsonSerializer.Serialize(requestBody);
            var requestContent = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Delete, $"{_apiUrl}{endpoint}")
            {
                Content = requestContent
            };

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var commitResponse = JsonSerializer.Deserialize<GitHubFileCommitResponse>(responseContent, _jsonOptions);

            if (commitResponse == null)
            {
                throw new Exception($"Failed to deserialize commit response for {path}");
            }

            return commitResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file {Path}", path);
            throw;
        }
    }

    /// <summary>
    /// Creates a new branch in the repository
    /// </summary>
    public async Task<GitHubBranch> CreateBranchAsync(string branchName, string baseBranch, CancellationToken cancellationToken = default!)
    {
        try
        {
            ValidateRepositoryConfig();

            // First, get the SHA of the latest commit on the base branch
            var refResponse = await _httpClient.GetAsync($"{_apiUrl}/repos/{_owner}/{_repository}/git/refs/heads/{baseBranch}", cancellationToken);
            refResponse.EnsureSuccessStatusCode();

            var refContent = await refResponse.Content.ReadAsStringAsync(cancellationToken);
            var refData = JsonSerializer.Deserialize<GitHubRef>(refContent, _jsonOptions);

            if (refData == null || refData.Object == null)
            {
                throw new Exception($"Failed to get reference for branch {baseBranch}");
            }

            // Create the new reference (branch)
            var requestBody = new
            {
                @ref = $"refs/heads/{branchName}",
                sha = refData.Object.Sha
            };

            var json = JsonSerializer.Serialize(requestBody);
            var requestContent = new StringContent(json, Encoding.UTF8, "application/json");

            var createResponse = await _httpClient.PostAsync($"{_apiUrl}/repos/{_owner}/{_repository}/git/refs", requestContent, cancellationToken);
            createResponse.EnsureSuccessStatusCode();

            // Now get the details of the new branch
            var branchResponse = await _httpClient.GetAsync($"{_apiUrl}/repos/{_owner}/{_repository}/branches/{branchName}");
            branchResponse.EnsureSuccessStatusCode();

            var branchContent = await branchResponse.Content.ReadAsStringAsync(cancellationToken);
            var branch = JsonSerializer.Deserialize<GitHubBranch>(branchContent, _jsonOptions);

            if (branch == null)
            {
                throw new Exception($"Failed to get details for new branch {branchName}");
            }

            return branch;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating branch {BranchName} from {BaseBranch}", branchName, baseBranch);
            throw;
        }
    }

    /// <summary>
    /// Creates a new pull request in the repository
    /// </summary>
    public async Task<GitHubPullRequest> CreatePullRequestAsync(string title, string body, string head, string baseBranch, CancellationToken cancellationToken = default!)
    {
        try
        {
            ValidateRepositoryConfig();

            var requestBody = new
            {
                title,
                body,
                head,
                @base = baseBranch
            };

            var json = JsonSerializer.Serialize(requestBody);
            var requestContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_apiUrl}/repos/{_owner}/{_repository}/pulls", requestContent, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var pullRequest = JsonSerializer.Deserialize<GitHubPullRequest>(responseContent, _jsonOptions);

            if (pullRequest == null)
            {
                throw new Exception("Failed to deserialize pull request response");
            }

            return pullRequest;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating pull request from {Head} to {BaseBranch}", head, baseBranch);
            throw;
        }
    }

    /// <summary>
    /// Searches for files in the repository
    /// </summary>
    public async Task<GitHubSearchResult> SearchFilesAsync(string query, CancellationToken cancellationToken = default!)
    {
        try
        {
            ValidateRepositoryConfig();

            // Construct the search query
            var searchQuery = $"repo:{_owner}/{_repository}";

            if (!string.IsNullOrEmpty(_branch))
            {
                searchQuery += $"+branch:{_branch}";
            }

            if (!string.IsNullOrEmpty(query))
            {
                searchQuery += $"+{Uri.EscapeDataString(query)}";
            }

            var response = await _httpClient.GetAsync($"{_apiUrl}/search/code?q={searchQuery}", cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var searchResult = JsonSerializer.Deserialize<GitHubSearchResult>(content, _jsonOptions);

            return searchResult ?? new GitHubSearchResult { Items = new List<GitHubItem>() };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching files with query {Query}", query);
            throw;
        }
    }

    /// <summary>
    /// Sets the owner and repository for subsequent requests
    /// </summary>
    public void SetRepository(string owner, string repository)
    {
        _owner = owner;
        _repository = repository;
    }

    /// <summary>
    /// Sets the branch for subsequent requests
    /// </summary>
    public void SetBranch(string branch)
    {
        _branch = branch;
    }

    /// <summary>
    /// Sets the access token for authentication
    /// </summary>
    public void SetAccessToken(string token)
    {
        _accessToken = token;
        ConfigureHttpClient();
    }

    /// <summary>
    /// Configures the HTTP client with authentication headers
    /// </summary>
    private void ConfigureHttpClient()
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
        _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("OsirionBlogCMS", "2.0"));

        if (!string.IsNullOrEmpty(_accessToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", _accessToken);
        }
    }

    /// <summary>
    /// Validates that repository configuration is set
    /// </summary>
    private void ValidateRepositoryConfig()
    {
        if (string.IsNullOrEmpty(_owner) || string.IsNullOrEmpty(_repository))
        {
            throw new InvalidOperationException("Repository owner and name must be set before making requests");
        }

        if (string.IsNullOrEmpty(_accessToken))
        {
            throw new UnauthorizedAccessException("Access token is required for GitHub API requests");
        }
    }
}

/// <summary>
/// Model for GitHub reference (used for branch creation)
/// </summary>
public class GitHubRef
{
    public string Ref { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public GitHubRefObject? Object { get; set; }
}

/// <summary>
/// Model for GitHub reference object (used for branch creation)
/// </summary>
public class GitHubRefObject
{
    public string Sha { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}