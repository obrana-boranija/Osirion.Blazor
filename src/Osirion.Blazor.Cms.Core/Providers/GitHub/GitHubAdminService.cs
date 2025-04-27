using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Core.Interfaces;
using Osirion.Blazor.Cms.Core.Models;
using Osirion.Blazor.Cms.Core.Providers.Interfaces;
using Osirion.Blazor.Cms.Infrastructure.GitHub.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Osirion.Blazor.Cms.Core.Providers.GitHub;

/// <summary>
/// Implementation of IGitHubAdminService for managing GitHub repositories
/// </summary>
public class GitHubAdminService : IGitHubAdminService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GitHubAdminService> _logger;
    private readonly string _owner;
    private string _repo = string.Empty;
    private string _branch = "main";
    private readonly string? _committerName;
    private readonly string? _committerEmail;
    private readonly IAuthenticationService _authService;

    public string CurrentRepository => _repo;
    public string CurrentBranch => _branch;

    public GitHubAdminService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<GitHubAdminService> logger,
        IAuthenticationService authService)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));

        // Read GitHub configuration
        _owner = configuration["GitHub:Owner"] ?? throw new ArgumentNullException("GitHub:Owner configuration is missing");
        _repo = configuration["GitHub:Repository"] ?? string.Empty;
        _branch = configuration["GitHub:Branch"] ?? "main";
        _committerName = configuration["GitHub:CommitterName"];
        _committerEmail = configuration["GitHub:CommitterEmail"];

        // Set up HTTP client
        _httpClient.BaseAddress = new Uri("https://api.github.com/");
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "OsirionBlogCMS");

        // Add authorization if user is authenticated
        if (_authService.IsAuthenticated && !string.IsNullOrEmpty(_authService.AccessToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", _authService.AccessToken);
        }
    }

    /// <summary>
    /// Sets the GitHub API token for authentication
    /// </summary>
    public Task SetAuthTokenAsync(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
        else
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", token);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Sets the current repository for operations
    /// </summary>
    public void SetRepository(string repository)
    {
        _repo = repository;
    }

    /// <summary>
    /// Sets the current branch for operations
    /// </summary>
    public void SetBranch(string branch)
    {
        _branch = branch;
    }

    /// <summary>
    /// Gets repositories for the authenticated user or configured owner
    /// </summary>
    public async Task<List<GitHubRepository>> GetRepositoriesAsync()
    {
        try
        {
            // Try to get user repos first (if authenticated)
            var endpoint = "user/repos?sort=updated&per_page=100";

            var response = await _httpClient.GetAsync(endpoint);

            // If unauthorized, fall back to public repos for the owner
            if (!response.IsSuccessStatusCode && response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                endpoint = $"users/{_owner}/repos?sort=updated&per_page=100";
                response = await _httpClient.GetAsync(endpoint);
            }

            response.EnsureSuccessStatusCode();

            var repositories = await response.Content.ReadFromJsonAsync<List<GitHubRepository>>();
            return repositories ?? new List<GitHubRepository>();
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    /// <summary>
    /// Gets branches for a repository
    /// </summary>
    public async Task<List<GitHubBranch>> GetBranchesAsync(string repository)
    {
        try
        {
            var endpoint = $"repos/{_owner}/{repository}/branches";

            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var branches = await response.Content.ReadFromJsonAsync<List<GitHubBranch>>();
            return branches ?? new List<GitHubBranch>();
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    /// <summary>
    /// Creates a new branch in the repository
    /// </summary>
    public async Task<GitHubBranch> CreateBranchAsync(string branchName, string fromBranch)
    {
        try
        {
            // First, get the SHA of the branch we're branching from
            var endpoint = $"repos/{_owner}/{_repo}/branches/{fromBranch}";

            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var sourceBranch = await response.Content.ReadFromJsonAsync<GitHubBranch>();
            if (sourceBranch == null)
            {
                throw new Exception($"Failed to get branch {fromBranch}");
            }

            // Create the new branch
            endpoint = $"repos/{_owner}/{_repo}/git/refs";

            var requestData = new
            {
                @ref = $"refs/heads/{branchName}",
                sha = sourceBranch.Commit.Sha
            };

            var requestJson = JsonSerializer.Serialize(requestData);
            var requestContent = new StringContent(requestJson, Encoding.UTF8);
            requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            response = await _httpClient.PostAsync(endpoint, requestContent);
            response.EnsureSuccessStatusCode();

            // Get the new branch
            endpoint = $"repos/{_owner}/{_repo}/branches/{branchName}";

            response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var newBranch = await response.Content.ReadFromJsonAsync<GitHubBranch>();
            return newBranch ?? new GitHubBranch();
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    /// <summary>
    /// Gets contents of a directory in the repository
    /// </summary>
    public async Task<List<GitHubItem>> GetRepositoryContentsAsync(string path = "")
    {
        try
        {
            var endpoint = $"repos/{_owner}/{_repo}/contents/{path}";
            if (!string.IsNullOrEmpty(_branch))
            {
                endpoint += $"?ref={_branch}";
            }

            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            // Directory content returns an array
            if (response.Content.Headers.ContentType?.MediaType == "application/json")
            {
                var content = await response.Content.ReadAsStringAsync();

                // Check if response is an array (directory) or single object (file)
                if (content.TrimStart().StartsWith("["))
                {
                    var items = await response.Content.ReadFromJsonAsync<List<GitHubItem>>();
                    return items ?? new List<GitHubItem>();
                }
                else
                {
                    // Single file content, return as a list with one item
                    var item = await response.Content.ReadFromJsonAsync<GitHubItem>();
                    return item != null ? new List<GitHubItem> { item } : new List<GitHubItem>();
                }
            }

            return new List<GitHubItem>();
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    /// <summary>
    /// Gets content of a file from the repository
    /// </summary>
    public async Task<GitHubFileContent> GetFileContentAsync(string path)
    {
        try
        {
            var endpoint = $"repos/{_owner}/{_repo}/contents/{path}";
            if (!string.IsNullOrEmpty(_branch))
            {
                endpoint += $"?ref={_branch}";
            }

            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<GitHubFileContent>();
            return content ?? new GitHubFileContent();
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    /// <summary>
    /// Gets content of a markdown file and parses it as a blog post
    /// </summary>
    public async Task<BlogPost> GetBlogPostAsync(string path)
    {
        var fileContent = await GetFileContentAsync(path);
        return BlogPost.FromGitHubFile(fileContent);
    }

    /// <summary>
    /// Creates a new file or updates an existing file in the repository
    /// </summary>
    public async Task<GitHubFileCommitResponse> CreateOrUpdateFileAsync(
        string path,
        string content,
        string commitMessage,
        string? existingSha = null)
    {
        try
        {
            var endpoint = $"repos/{_owner}/{_repo}/contents/{path}";

            // Fix: Properly handle content encoding to avoid 422 errors
            // Convert content to Base64 using UTF-8 encoding first
            var contentBytes = Encoding.UTF8.GetBytes(content);
            var base64Content = Convert.ToBase64String(contentBytes);

            var requestData = new
            {
                message = commitMessage,
                content = base64Content,
                branch = _branch,
                sha = existingSha,
                committer = !string.IsNullOrEmpty(_committerName) && !string.IsNullOrEmpty(_committerEmail) ? new
                {
                    name = _committerName,
                    email = _committerEmail
                } : null
            };

            var requestJson = JsonSerializer.Serialize(requestData, new JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            _logger.LogInformation("Request payload for file operation: {RequestJson}", requestJson);

            var requestContent = new StringContent(requestJson, Encoding.UTF8);
            requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _httpClient.PutAsync(endpoint, requestContent);

            // Log the full response for debugging
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Response from GitHub: Status: {StatusCode}, Content: {ResponseContent}",
                                  response.StatusCode, responseContent);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<GitHubFileCommitResponse>();
            return result ?? new GitHubFileCommitResponse();
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    /// <summary>
    /// Deletes a file from the repository
    /// </summary>
    public async Task<GitHubFileCommitResponse> DeleteFileAsync(
        string path,
        string commitMessage,
        string sha)
    {
        try
        {
            var endpoint = $"repos/{_owner}/{_repo}/contents/{path}";

            var requestData = new
            {
                message = commitMessage,
                sha,
                branch = _branch,
                committer = !string.IsNullOrEmpty(_committerName) && !string.IsNullOrEmpty(_committerEmail) ? new
                {
                    name = _committerName,
                    email = _committerEmail
                } : null
            };

            var requestJson = JsonSerializer.Serialize(requestData, new JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            var request = new HttpRequestMessage(HttpMethod.Delete, endpoint)
            {
                Content = new StringContent(requestJson, Encoding.UTF8)
            };
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<GitHubFileCommitResponse>();
            return result ?? new GitHubFileCommitResponse();
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    /// <summary>
    /// Searches for files in the repository
    /// </summary>
    public async Task<List<GitHubItem>> SearchFilesAsync(string query)
    {
        try
        {
            // GitHub's search API for code
            var endpoint = $"search/code?q={Uri.EscapeDataString(query)}+repo:{_owner}/{_repo}";
            if (!string.IsNullOrEmpty(_branch))
            {
                endpoint += $"+ref:{_branch}";
            }

            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var searchResult = await response.Content.ReadFromJsonAsync<GitHubSearchResult>();

            var results = new List<GitHubItem>();

            if (searchResult?.Items != null)
            {
                foreach (var item in searchResult.Items)
                {
                    // Get the full item details
                    try
                    {
                        var fullItem = await GetFileContentAsync(item.Path);
                        if (fullItem != null)
                        {
                            var gitHubItem = new GitHubItem
                            {
                                Name = fullItem.Name,
                                Path = fullItem.Path,
                                Sha = fullItem.Sha,
                                Size = fullItem.Size,
                                Url = fullItem.Url,
                                Type = "file",
                                DownloadUrl = fullItem.DownloadUrl
                            };

                            results.Add(gitHubItem);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error getting full item details for search result: {Path}", item.Path);
                    }
                }
            }

            return results;
        }
        catch (Exception ex)
        {
            return new List<GitHubItem>();
        }
    }

    /// <summary>
    /// Creates a pull request
    /// </summary>
    public async Task<GitHubPullRequest> CreatePullRequestAsync(string title, string body, string head, string baseBranch)
    {
        try
        {
            var endpoint = $"repos/{_owner}/{_repo}/pulls";

            var requestData = new
            {
                title,
                body,
                head,
                @base = baseBranch
            };

            var requestJson = JsonSerializer.Serialize(requestData);
            var requestContent = new StringContent(requestJson, Encoding.UTF8);
            requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _httpClient.PostAsync(endpoint, requestContent);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<GitHubPullRequest>();
            return result ?? new GitHubPullRequest();
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}