using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Osirion.Blazor.Cms.Infrastructure.GitHub;

/// <summary>
/// GitHub API client implementation
/// </summary>
/// <remarks>
/// Owner, repository, branch and access token belong to this instance only. They are held in an immutable
/// snapshot that the Set* methods replace as a whole, and the Accept, User-Agent and Authorization headers
/// are set on each <see cref="HttpRequestMessage"/>. The <see cref="HttpClient.DefaultRequestHeaders"/> of
/// the underlying client are never modified, so two instances that share an <see cref="HttpClient"/> never
/// see each other's token, repository or branch.
/// </remarks>
public class GitHubApiClient : IGitHubApiClient
{
    private const string DefaultApiUrl = "https://api.github.com";
    private const string GitHubMediaType = "application/vnd.github.v3+json";
    private const string UserAgentProduct = "OsirionBlogCMS";
    private const string UserAgentVersion = "2.0";
    private const string AuthorizationScheme = "token";
    private const string AcceptHeaderName = "Accept";
    private const string UserAgentHeaderName = "User-Agent";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly Func<HttpClient> _acquireHttpClient;
    private readonly ILogger<GitHubApiClient> _logger;
    private readonly Lock _settingsLock = new();
    private ClientSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="GitHubApiClient"/> class
    /// </summary>
    /// <param name="httpClient">The HTTP client to use for API requests</param>
    /// <param name="options">GitHub API configuration options</param>
    /// <param name="logger">The logger instance</param>
    /// <exception cref="ArgumentNullException">Thrown if required dependencies are null</exception>
    public GitHubApiClient(
    HttpClient httpClient,
    IOptions<GitHubOptions> options,
    ILogger<GitHubApiClient> logger)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        _acquireHttpClient = () => httpClient;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settings = ClientSettings.From(options.Value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GitHubApiClient"/> class that takes a short-lived
    /// <see cref="HttpClient"/> from <paramref name="httpClientFactory"/> for every operation
    /// </summary>
    /// <param name="httpClientFactory">The factory that creates the named HTTP clients</param>
    /// <param name="httpClientName">The name of the HTTP client to create</param>
    /// <param name="options">GitHub API configuration options</param>
    /// <param name="logger">The logger instance</param>
    /// <exception cref="ArgumentNullException">Thrown if required dependencies are null</exception>
    internal GitHubApiClient(
        IHttpClientFactory httpClientFactory,
        string httpClientName,
        GitHubOptions options,
        ILogger<GitHubApiClient> logger)
    {
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        ArgumentNullException.ThrowIfNull(httpClientName);
        ArgumentNullException.ThrowIfNull(options);

        _acquireHttpClient = () => httpClientFactory.CreateClient(httpClientName);
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settings = ClientSettings.From(options);
    }

    private ClientSettings CurrentSettings => Volatile.Read(ref _settings);

    /// <summary>
    /// Gets a list of repositories the authenticated user has access to
    /// </summary>
    public async Task<List<GitHubRepository>?> GetRepositoriesAsync(CancellationToken cancellationToken = default!)
    {
        var settings = CurrentSettings;

        try
        {
            // Check if we have an access token
            if (string.IsNullOrWhiteSpace(settings.AccessToken))
            {
                throw new UnauthorizedAccessException("Access token is required to list repositories");
            }

            // Prepare the request: the authenticated user's repos, or the organization's repos when the owner is an organization
            string endpoint;

            // If the owner is set, try to get repositories for that organization/user
            if (!string.IsNullOrWhiteSpace(settings.Owner))
            {
                var owner = GitHubApiPath.EncodeSegment(settings.Owner, nameof(settings.Owner));
                endpoint = $"/users/{owner}/repos";

                // Check if the owner is an organization
                try
                {
                    using var orgResponse = await SendAsync(HttpMethod.Get, $"{settings.ApiUrl}/orgs/{owner}", settings, cancellationToken);
                    if (orgResponse.IsSuccessStatusCode)
                    {
                        endpoint = $"/orgs/{owner}/repos";
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
            var content = await GetResponseBodyAsync(HttpMethod.Get, $"{settings.ApiUrl}{endpoint}", settings, cancellationToken);
            var repositories = JsonSerializer.Deserialize<List<GitHubRepository>>(content, JsonOptions);

            return repositories ?? [];
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
    public async Task<List<GitHubBranch>?> GetBranchesAsync(CancellationToken cancellationToken = default!)
    {
        var settings = CurrentSettings;

        try
        {
            if (!ValidateRepositoryConfig(settings))
            {
                return null;
            }

            var content = await GetResponseBodyAsync(HttpMethod.Get, $"{GetRepositoryUrl(settings)}/branches", settings, cancellationToken);
            var branches = JsonSerializer.Deserialize<List<GitHubBranch>>(content, JsonOptions);

            return branches ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching branches for repository {Owner}/{Repository}", settings.Owner, settings.Repository);
            throw;
        }
    }

    /// <summary>
    /// Gets the contents of the repository at the specified path
    /// </summary>
    /// <exception cref="ArgumentException">The path contains an empty, "." or ".." segment.</exception>
    public async Task<List<GitHubItem>?> GetRepositoryContentsAsync(string path, CancellationToken cancellationToken = default!)
    {
        var settings = CurrentSettings;

        try
        {
            if (!ValidateRepositoryConfig(settings))
            {
                return null;
            }

            var url = WithBranchReference(GetContentsUrl(settings, path), settings);
            var content = await GetResponseBodyAsync(HttpMethod.Get, url, settings, cancellationToken);

            // GitHub API can return either a single item or an array of items
            try
            {
                // Try to deserialize as array first
                var items = JsonSerializer.Deserialize<List<GitHubItem>>(content, JsonOptions);
                return items ?? [];
            }
            catch
            {
                // If that fails, try to deserialize as a single item
                var item = JsonSerializer.Deserialize<GitHubItem>(content, JsonOptions);
                return item is not null ? [item] : [];
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
    /// <exception cref="ArgumentException">The path contains an empty, "." or ".." segment.</exception>
    public async Task<GitHubFileContent?> GetFileContentAsync(string path, CancellationToken cancellationToken = default!)
    {
        var settings = CurrentSettings;

        try
        {
            if (!ValidateRepositoryConfig(settings))
            {
                return null;
            }

            var url = WithBranchReference(GetContentsUrl(settings, path), settings);
            var content = await GetResponseBodyAsync(HttpMethod.Get, url, settings, cancellationToken);
            var fileContent = JsonSerializer.Deserialize<GitHubFileContent>(content, JsonOptions);

            if (fileContent is null)
            {
                _logger.LogError($"Failed to deserialize file content for {path}");
            }

            return fileContent ?? new();
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
    /// <exception cref="ArgumentException">The path contains an empty, "." or ".." segment.</exception>
    public async Task<GitHubFileCommitResponse?> CreateOrUpdateFileAsync(string path, string content, string message, string? sha = null, CancellationToken cancellationToken = default!)
    {
        var settings = CurrentSettings;

        try
        {
            if (!ValidateRepositoryConfig(settings))
            {
                return null;
            }

            var url = GetContentsUrl(settings, path);

            // Convert content to base64
            var contentBytes = Encoding.UTF8.GetBytes(content);
            var base64Content = Convert.ToBase64String(contentBytes);

            // Create request body
            var requestBody = new
            {
                message,
                content = base64Content,
                branch = settings.Branch,
                sha // Include SHA only for updates
            };

            var json = JsonSerializer.Serialize(requestBody);
            var requestContent = new StringContent(json, Encoding.UTF8, "application/json");

            var responseContent = await GetResponseBodyAsync(HttpMethod.Put, url, settings, cancellationToken, requestContent);
            var commitResponse = JsonSerializer.Deserialize<GitHubFileCommitResponse>(responseContent, JsonOptions);

            return commitResponse is null ? throw new Exception($"Failed to deserialize commit response for {path}") : commitResponse;
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
    /// <exception cref="ArgumentException">The path contains an empty, "." or ".." segment.</exception>
    /// <exception cref="InvalidOperationException">
    /// The owner, repository or access token is not set. No request is sent, so a token configured on the
    /// underlying <see cref="HttpClient"/> can never perform the delete.
    /// </exception>
    public async Task<GitHubFileCommitResponse> DeleteFileAsync(string path, string message, string sha, CancellationToken cancellationToken = default!)
    {
        var settings = CurrentSettings;

        try
        {
            if (!ValidateRepositoryConfig(settings))
            {
                throw new InvalidOperationException("Repository owner, name and access token must be set before deleting a file.");
            }

            var url = GetContentsUrl(settings, path);

            // Create request body
            var requestBody = new
            {
                message,
                sha,
                branch = settings.Branch
            };

            var json = JsonSerializer.Serialize(requestBody);
            var requestContent = new StringContent(json, Encoding.UTF8, "application/json");

            var responseContent = await GetResponseBodyAsync(HttpMethod.Delete, url, settings, cancellationToken, requestContent);
            var commitResponse = JsonSerializer.Deserialize<GitHubFileCommitResponse>(responseContent, JsonOptions);

            return commitResponse is null ? throw new Exception($"Failed to deserialize commit response for {path}") : commitResponse;
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
    /// <exception cref="ArgumentException">A branch name is empty or contains an empty, "." or ".." segment.</exception>
    public async Task<GitHubBranch?> CreateBranchAsync(string branchName, string baseBranch, CancellationToken cancellationToken = default!)
    {
        var settings = CurrentSettings;

        try
        {
            if (!ValidateRepositoryConfig(settings))
            {
                return null;
            }

            // Encode both names before the first request, so an invalid name never sends anything
            var repositoryUrl = GetRepositoryUrl(settings);
            var baseBranchUrl = $"{repositoryUrl}/git/refs/heads/{GitHubApiPath.EncodeBranchPath(baseBranch, nameof(baseBranch))}";
            var newBranchUrl = $"{repositoryUrl}/branches/{GitHubApiPath.EncodeBranchPath(branchName, nameof(branchName))}";

            // First, get the SHA of the latest commit on the base branch
            var refContent = await GetResponseBodyAsync(HttpMethod.Get, baseBranchUrl, settings, cancellationToken);
            var refData = JsonSerializer.Deserialize<GitHubRef>(refContent, JsonOptions);

            if (refData is null || refData.Object is null)
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

            await GetResponseBodyAsync(HttpMethod.Post, $"{repositoryUrl}/git/refs", settings, cancellationToken, requestContent);

            // Now get the details of the new branch
            var branchContent = await GetResponseBodyAsync(HttpMethod.Get, newBranchUrl, settings, cancellationToken);
            var branch = JsonSerializer.Deserialize<GitHubBranch>(branchContent, JsonOptions);

            return branch is null ? throw new Exception($"Failed to get details for new branch {branchName}") : branch;
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
    public async Task<GitHubPullRequest?> CreatePullRequestAsync(string title, string body, string head, string baseBranch, CancellationToken cancellationToken = default!)
    {
        var settings = CurrentSettings;

        try
        {
            if (!ValidateRepositoryConfig(settings))
            {
                return null;
            }

            var requestBody = new
            {
                title,
                body,
                head,
                @base = baseBranch
            };

            var json = JsonSerializer.Serialize(requestBody);
            var requestContent = new StringContent(json, Encoding.UTF8, "application/json");

            var responseContent = await GetResponseBodyAsync(HttpMethod.Post, $"{GetRepositoryUrl(settings)}/pulls", settings, cancellationToken, requestContent);
            var pullRequest = JsonSerializer.Deserialize<GitHubPullRequest>(responseContent, JsonOptions);

            return pullRequest is null ? throw new Exception("Failed to deserialize pull request response") : pullRequest;
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
    public async Task<GitHubSearchResult?> SearchFilesAsync(string query, CancellationToken cancellationToken = default!)
    {
        var settings = CurrentSettings;

        try
        {
            if (!ValidateRepositoryConfig(settings))
            {
                return null;
            }

            // Construct the search query
            var owner = GitHubApiPath.EncodeSegment(settings.Owner, nameof(settings.Owner));
            var repository = GitHubApiPath.EncodeSegment(settings.Repository, nameof(settings.Repository));
            var searchQuery = $"repo:{owner}/{repository}";

            if (!string.IsNullOrWhiteSpace(settings.Branch))
            {
                searchQuery += $"+branch:{GitHubApiPath.EncodeQueryValue(settings.Branch)}";
            }

            if (!string.IsNullOrWhiteSpace(query))
            {
                searchQuery += $"+{Uri.EscapeDataString(query)}";
            }

            var content = await GetResponseBodyAsync(HttpMethod.Get, $"{settings.ApiUrl}/search/code?q={searchQuery}", settings, cancellationToken);
            var searchResult = JsonSerializer.Deserialize<GitHubSearchResult>(content, JsonOptions);

            return searchResult ?? new GitHubSearchResult { Items = [] };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching files with query {Query}", query);
            throw;
        }
    }

    /// <summary>
    /// Sets the owner and repository for subsequent requests made through this instance only
    /// </summary>
    public void SetRepository(string owner, string repository)
    {
        UpdateSettings(settings => settings with { Owner = owner, Repository = repository });
    }

    /// <summary>
    /// Sets the branch for subsequent requests made through this instance only
    /// </summary>
    public void SetBranch(string branch)
    {
        UpdateSettings(settings => settings with { Branch = branch });
    }

    /// <summary>
    /// Sets the access token for subsequent requests made through this instance only
    /// </summary>
    /// <remarks>
    /// The token is sent on each request message and the underlying <see cref="HttpClient"/> is never changed.
    /// An empty token removes authentication from this instance.
    /// </remarks>
    public void SetAccessToken(string token)
    {
        UpdateSettings(settings => settings with { AccessToken = token ?? string.Empty });
    }

    private void UpdateSettings(Func<ClientSettings, ClientSettings> update)
    {
        lock (_settingsLock)
        {
            Volatile.Write(ref _settings, update(_settings));
        }
    }

    private static string GetRepositoryUrl(ClientSettings settings)
    {
        var owner = GitHubApiPath.EncodeSegment(settings.Owner, nameof(settings.Owner));
        var repository = GitHubApiPath.EncodeSegment(settings.Repository, nameof(settings.Repository));
        return $"{settings.ApiUrl}/repos/{owner}/{repository}";
    }

    private static string GetContentsUrl(ClientSettings settings, string? path)
    {
        return $"{GetRepositoryUrl(settings)}/contents/{GitHubApiPath.EncodePath(path, nameof(path))}";
    }

    private static string WithBranchReference(string url, ClientSettings settings)
    {
        return string.IsNullOrWhiteSpace(settings.Branch)
            ? url
            : $"{url}?ref={GitHubApiPath.EncodeQueryValue(settings.Branch)}";
    }

    /// <summary>
    /// Sends a request and returns the response body, throwing for a non-success status code
    /// </summary>
    private async Task<string> GetResponseBodyAsync(
        HttpMethod method,
        string url,
        ClientSettings settings,
        CancellationToken cancellationToken,
        HttpContent? content = null)
    {
        using var response = await SendAsync(method, url, settings, cancellationToken, content);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    /// <summary>
    /// Sends a request whose headers come from <paramref name="settings"/>, never from shared client state
    /// </summary>
    private async Task<HttpResponseMessage> SendAsync(
        HttpMethod method,
        string url,
        ClientSettings settings,
        CancellationToken cancellationToken,
        HttpContent? content = null)
    {
        var httpClient = _acquireHttpClient();

        using var request = new HttpRequestMessage(method, url) { Content = content };
        ApplyHeaders(request, httpClient, settings);

        return await httpClient.SendAsync(request, cancellationToken);
    }

    private static void ApplyHeaders(HttpRequestMessage request, HttpClient httpClient, ClientSettings settings)
    {
        // Accept and User-Agent configured by the host on the HttpClient take precedence
        if (!httpClient.DefaultRequestHeaders.Contains(AcceptHeaderName))
        {
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(GitHubMediaType));
        }

        if (!httpClient.DefaultRequestHeaders.Contains(UserAgentHeaderName))
        {
            request.Headers.UserAgent.Add(new ProductInfoHeaderValue(UserAgentProduct, UserAgentVersion));
        }

        if (!string.IsNullOrWhiteSpace(settings.AccessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(AuthorizationScheme, settings.AccessToken);
        }
    }

    /// <summary>
    /// Validates that repository configuration is set
    /// </summary>
    private bool ValidateRepositoryConfig(ClientSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.Owner) || string.IsNullOrWhiteSpace(settings.Repository))
        {
            _logger.LogError("Repository owner and name must be set before making requests");
            return false;
        }

        if (string.IsNullOrWhiteSpace(settings.AccessToken))
        {
            _logger.LogError("Access token is required for GitHub API requests");
            return false;
        }

        return true;
    }

    /// <summary>
    /// The values a request needs, replaced as a whole so a request never mixes values from two updates
    /// </summary>
    private sealed record ClientSettings(string Owner, string Repository, string Branch, string AccessToken, string ApiUrl)
    {
        public static ClientSettings From(GitHubOptions options)
        {
            return new ClientSettings(
                options.Owner,
                options.Repository,
                options.Branch,
                ResolveAccessToken(options),
                options.ApiUrl ?? DefaultApiUrl);
        }

        private static string ResolveAccessToken(GitHubOptions options)
        {
            // A GitHubProviderOptions instance can carry its own personal access token
            if (options is Domain.Options.Configuration.GitHubProviderOptions providerOptions)
            {
                return providerOptions.Authentication?.PersonalAccessToken ?? options.ApiToken ?? string.Empty;
            }

            // Plain GitHubOptions use the configured API token
            return options.ApiToken ?? string.Empty;
        }
    }
}

/// <summary>
/// Model for GitHub reference (used for branch creation)
/// </summary>
public class GitHubRef
{
    /// <summary>
    /// Gets or sets the reference name
    /// </summary>
    public string Ref { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the reference URL
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the reference object
    /// </summary>
    public GitHubRefObject? Object { get; set; }
}

/// <summary>
/// Model for GitHub reference object (used for branch creation)
/// </summary>
public class GitHubRefObject
{
    /// <summary>
    /// Gets or sets the SHA hash
    /// </summary>
    public string Sha { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the object type
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the object URL
    /// </summary>
    public string Url { get; set; } = string.Empty;
}
