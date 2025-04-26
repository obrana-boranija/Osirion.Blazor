using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Exceptions;
using Osirion.Blazor.Cms.Options;
using Osirion.Blazor.Cms.Providers.GitHub.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Osirion.Blazor.Cms.Providers.GitHub;

/// <summary>
/// Client for interacting with the GitHub API
/// </summary>
public interface IGitHubApiClient
{
    /// <summary>
    /// Gets the content of a repository at the specified path
    /// </summary>
    Task<List<GitHubItem>> GetRepositoryContentsAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the content of a file
    /// </summary>
    Task<GitHubFileContent> GetFileContentAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets information about a file from the commit history
    /// </summary>
    Task<(DateTime Created, DateTime? Modified)> GetFileHistoryAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all branches in the repository
    /// </summary>
    Task<List<GitHubBranch>> GetBranchesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates or updates a file in the repository
    /// </summary>
    Task<GitHubFileCommitResponse> CreateOrUpdateFileAsync(
        string path,
        string content,
        string message,
        string? sha = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file from the repository
    /// </summary>
    Task<GitHubFileCommitResponse> DeleteFileAsync(
        string path,
        string message,
        string sha,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for files in the repository
    /// </summary>
    Task<GitHubSearchResult> SearchFilesAsync(string query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the access token for authenticated requests
    /// </summary>
    void SetAccessToken(string token);

    /// <summary>
    /// Sets the repository for API operations
    /// </summary>
    void SetRepository(string owner, string repo);

    /// <summary>
    /// Sets the branch for API operations
    /// </summary>
    void SetBranch(string branch);
}

/// <summary>
/// Implementation of the GitHub API client
/// </summary>
public class GitHubApiClient : IGitHubApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GitHubApiClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    private string _owner;
    private string _repo;
    private string _branch;
    private string? _committerName;
    private string? _committerEmail;
    private static readonly SemaphoreSlim _rateLimitLock = new(1, 1);

    /// <summary>
    /// Initializes a new instance of the GitHubApiClient class
    /// </summary>
    public GitHubApiClient(
        HttpClient httpClient,
        IOptions<GitHubContentOptions> options,
        ILogger<GitHubApiClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        var optionsValue = options?.Value ?? throw new ArgumentNullException(nameof(options));

        _owner = optionsValue.Owner;
        _repo = optionsValue.Repository;
        _branch = optionsValue.Branch;
        _committerName = null;
        _committerEmail = null;

        // Configure HTTP client
        _httpClient.BaseAddress = new Uri("https://api.github.com/");
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "OsirionBlogCMS");

        // Set API token if provided
        if (!string.IsNullOrEmpty(optionsValue.ApiToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", optionsValue.ApiToken);
        }

        // Configure JSON serialization options
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
    }

    /// <inheritdoc/>
    public void SetAccessToken(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
        else
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", token);
        }
    }

    /// <inheritdoc/>
    public void SetRepository(string owner, string repo)
    {
        _owner = owner;
        _repo = repo;
    }

    /// <inheritdoc/>
    public void SetBranch(string branch)
    {
        _branch = branch;
    }

    /// <inheritdoc/>
    public async Task<List<GitHubItem>> GetRepositoryContentsAsync(string path, CancellationToken cancellationToken = default)
    {
        return await ExecuteApiCallAsync<List<GitHubItem>>(
            $"repos/{_owner}/{_repo}/contents/{path}?ref={_branch}",
            HttpMethod.Get,
            null,
            "Failed to fetch repository contents",
            cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<GitHubFileContent> GetFileContentAsync(string path, CancellationToken cancellationToken = default)
    {
        return await ExecuteApiCallAsync<GitHubFileContent>(
            $"repos/{_owner}/{_repo}/contents/{path}?ref={_branch}",
            HttpMethod.Get,
            null,
            "Failed to fetch file content",
            cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<(DateTime Created, DateTime? Modified)> GetFileHistoryAsync(string path, CancellationToken cancellationToken = default)
    {
        // Get latest commit for the file
        var latestCommitUrl = $"repos/{_owner}/{_repo}/commits?path={path}&page=1&per_page=1";
        var latestCommits = await ExecuteApiCallAsync<List<GitHubCommitInfo>>(
            latestCommitUrl,
            HttpMethod.Get,
            null,
            "Failed to fetch latest commit",
            cancellationToken);

        var latestCommit = latestCommits.FirstOrDefault();
        DateTime? modifiedDate = latestCommit?.Committer?.Date;

        // Get first commit for the file
        var firstCommitUrl = $"repos/{_owner}/{_repo}/commits?path={path}&per_page=1";
        var allCommits = await ExecuteApiCallAsync<List<GitHubCommitInfo>>(
            firstCommitUrl,
            HttpMethod.Get,
            null,
            "Failed to fetch commit history",
            cancellationToken);

        var firstCommit = allCommits.LastOrDefault();
        DateTime createdDate = firstCommit?.Author?.Date ?? DateTime.UtcNow;

        return (createdDate, modifiedDate);
    }

    /// <inheritdoc/>
    public async Task<List<GitHubBranch>> GetBranchesAsync(CancellationToken cancellationToken = default)
    {
        return await ExecuteApiCallAsync<List<GitHubBranch>>(
            $"repos/{_owner}/{_repo}/branches",
            HttpMethod.Get,
            null,
            "Failed to fetch branches",
            cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<GitHubFileCommitResponse> CreateOrUpdateFileAsync(
        string path,
        string content,
        string message,
        string? sha = null,
        CancellationToken cancellationToken = default)
    {
        // Convert content to Base64
        var contentBytes = Encoding.UTF8.GetBytes(content);
        var base64Content = Convert.ToBase64String(contentBytes);

        var requestData = new GitHubFileCommitRequest
        {
            Message = message,
            Content = base64Content,
            Branch = _branch,
            Sha = sha
        };

        // Add committer info if available
        if (!string.IsNullOrEmpty(_committerName) && !string.IsNullOrEmpty(_committerEmail))
        {
            requestData.Committer = new GitHubCommitter
            {
                Name = _committerName,
                Email = _committerEmail
            };
        }

        return await ExecuteApiCallAsync<GitHubFileCommitResponse>(
            $"repos/{_owner}/{_repo}/contents/{path}",
            HttpMethod.Put,
            requestData,
            "Failed to create or update file",
            cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<GitHubFileCommitResponse> DeleteFileAsync(
        string path,
        string message,
        string sha,
        CancellationToken cancellationToken = default)
    {
        var requestData = new GitHubFileDeleteRequest
        {
            Message = message,
            Sha = sha,
            Branch = _branch
        };

        // Add committer info if available
        if (!string.IsNullOrEmpty(_committerName) && !string.IsNullOrEmpty(_committerEmail))
        {
            requestData.Committer = new GitHubCommitter
            {
                Name = _committerName,
                Email = _committerEmail
            };
        }

        return await ExecuteApiCallAsync<GitHubFileCommitResponse>(
            $"repos/{_owner}/{_repo}/contents/{path}",
            HttpMethod.Delete,
            requestData,
            "Failed to delete file",
            cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<GitHubSearchResult> SearchFilesAsync(string query, CancellationToken cancellationToken = default)
    {
        // GitHub's search API for code
        var encodedQuery = Uri.EscapeDataString($"{query} repo:{_owner}/{_repo}");
        if (!string.IsNullOrEmpty(_branch))
        {
            encodedQuery += $"+ref:{_branch}";
        }

        return await ExecuteApiCallAsync<GitHubSearchResult>(
            $"search/code?q={encodedQuery}",
            HttpMethod.Get,
            null,
            "Failed to search files",
            cancellationToken);
    }

    /// <summary>
    /// Helper method to execute API calls with rate limiting and error handling
    /// </summary>
    private async Task<T> ExecuteApiCallAsync<T>(
        string url,
        HttpMethod method,
        object? data,
        string errorMessage,
        CancellationToken cancellationToken)
        where T : class, new()
    {
        // Handle rate limiting
        await _rateLimitLock.WaitAsync(cancellationToken);
        try
        {
            // Create the request
            using var request = new HttpRequestMessage(method, url);

            // Add request body if provided
            if (data != null)
            {
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            // Send the request
            using var response = await _httpClient.SendAsync(request, cancellationToken);

            // Check for rate limiting
            if (response.Headers.TryGetValues("X-RateLimit-Remaining", out var values))
            {
                if (int.TryParse(values.FirstOrDefault(), out var remaining) && remaining < 5)
                {
                    _logger.LogWarning("GitHub API rate limit nearly reached. {Remaining} requests remaining.", remaining);
                }
            }

            // Handle response
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    // Check if response is an array when T is a list
                    if (typeof(T) == typeof(List<GitHubItem>) &&
                        !string.IsNullOrEmpty(content) &&
                        !content.TrimStart().StartsWith("["))
                    {
                        // Single item response when expecting a list
                        var item = JsonSerializer.Deserialize<GitHubItem>(content, _jsonOptions);
                        if (item != null)
                        {
                            var list = new List<GitHubItem> { item };
                            return list as T ?? new T();
                        }
                    }

                    var result = JsonSerializer.Deserialize<T>(content, _jsonOptions) ?? new T();

                    // Set success flag if response is GitHubApiResponse
                    if (result is GitHubApiResponse apiResponse)
                    {
                        apiResponse.Success = true;
                    }

                    return result;
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Failed to parse GitHub API response: {Content}", content);
                    throw new ContentProviderException($"{errorMessage}: Invalid JSON response");
                }
            }
            else
            {
                _logger.LogError("GitHub API error: {StatusCode} - {Content}", response.StatusCode, content);

                // Try to extract error message
                string detailedError = content;
                try
                {
                    var errorObj = JsonSerializer.Deserialize<JsonElement>(content);
                    if (errorObj.TryGetProperty("message", out var messageElement))
                    {
                        detailedError = messageElement.GetString() ?? detailedError;
                    }
                }
                catch
                {
                    // Ignore JSON parsing errors and use full content as error message
                }

                throw new ContentProviderException($"{errorMessage}: {response.StatusCode} - {detailedError}");
            }
        }
        catch (ContentProviderException)
        {
            // Re-throw our custom exceptions
            throw;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling GitHub API: {Url}", url);
            throw new ContentProviderException($"{errorMessage}: {ex.Message}", ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Timeout calling GitHub API: {Url}", url);
            throw new ContentProviderException($"{errorMessage}: Request timed out", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error calling GitHub API: {Url}", url);
            throw new ContentProviderException($"{errorMessage}: {ex.Message}", ex);
        }
        finally
        {
            _rateLimitLock.Release();
        }
    }
}