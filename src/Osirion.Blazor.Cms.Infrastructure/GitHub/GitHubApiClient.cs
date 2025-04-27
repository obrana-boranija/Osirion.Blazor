using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Domain.Models;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Infrastructure.GitHub.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Osirion.Blazor.Cms.Infrastructure.GitHub;

/// <summary>
/// Implementation of IGitHubApiClient
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

    public GitHubApiClient(
        HttpClient httpClient,
        IOptions<GitHubOptions> options,
        ILogger<GitHubApiClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        var optionsValue = options?.Value ?? throw new ArgumentNullException(nameof(options));

        _owner = optionsValue.Owner;
        _repo = optionsValue.Repository;
        _branch = optionsValue.Branch;
        _committerName = optionsValue.CommitterName;
        _committerEmail = optionsValue.CommitterEmail;

        // Configure HTTP client
        _httpClient.BaseAddress = new Uri(optionsValue.ApiUrl ?? "https://api.github.com/");
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

    public void SetRepository(string owner, string repo)
    {
        _owner = owner;
        _repo = repo;
    }

    public void SetBranch(string branch)
    {
        _branch = branch;
    }

    public async Task<List<GitHubItem>> GetRepositoryContentsAsync(string path, CancellationToken cancellationToken = default)
    {
        return await ExecuteApiCallAsync<List<GitHubItem>>(
            $"repos/{_owner}/{_repo}/contents/{path}?ref={_branch}",
            HttpMethod.Get,
            null,
            "Failed to fetch repository contents",
            cancellationToken);
    }

    public async Task<GitHubFileContent> GetFileContentAsync(string path, CancellationToken cancellationToken = default)
    {
        return await ExecuteApiCallAsync<GitHubFileContent>(
            $"repos/{_owner}/{_repo}/contents/{path}?ref={_branch}",
            HttpMethod.Get,
            null,
            "Failed to fetch file content",
            cancellationToken);
    }

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

    public async Task<List<GitHubBranch>> GetBranchesAsync(CancellationToken cancellationToken = default)
    {
        return await ExecuteApiCallAsync<List<GitHubBranch>>(
            $"repos/{_owner}/{_repo}/branches",
            HttpMethod.Get,
            null,
            "Failed to fetch branches",
            cancellationToken);
    }

    public async Task<GitHubBranch> CreateBranchAsync(string name, string fromBranch, CancellationToken cancellationToken = default)
    {
        // First, get the SHA of the source branch
        var sourceBranch = await ExecuteApiCallAsync<GitHubBranch>(
            $"repos/{_owner}/{_repo}/branches/{fromBranch}",
            HttpMethod.Get,
            null,
            $"Failed to get branch {fromBranch}",
            cancellationToken);

        // Create the new branch reference
        var reference = new
        {
            @ref = $"refs/heads/{name}",
            sha = sourceBranch.Commit.Sha
        };

        await ExecuteApiCallAsync<object>(
            $"repos/{_owner}/{_repo}/git/refs",
            HttpMethod.Post,
            reference,
            $"Failed to create branch {name}",
            cancellationToken);

        // Get the newly created branch
        return await ExecuteApiCallAsync<GitHubBranch>(
            $"repos/{_owner}/{_repo}/branches/{name}",
            HttpMethod.Get,
            null,
            $"Failed to get newly created branch {name}",
            cancellationToken);
    }

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

    public async Task<GitHubPullRequest> CreatePullRequestAsync(
        string title,
        string body,
        string head,
        string baseBranch,
        CancellationToken cancellationToken = default)
    {
        var requestData = new
        {
            title,
            body,
            head,
            @base = baseBranch
        };

        return await ExecuteApiCallAsync<GitHubPullRequest>(
            $"repos/{_owner}/{_repo}/pulls",
            HttpMethod.Post,
            requestData,
            "Failed to create pull request",
            cancellationToken);
    }

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