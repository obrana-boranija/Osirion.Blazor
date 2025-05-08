using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Infrastructure.Services;

/// <summary>
/// Service for GitHub authentication with repository adapter support
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly IStateStorageService _stateStorage;
    private readonly IGitHubTokenProvider _tokenProvider;
    private readonly IGitHubApiClient _apiClient;
    private readonly GitHubAdminOptions _githubOptions;
    private readonly AuthenticationOptions _authOptions;

    private string? _accessToken;
    private string? _username;

    /// <summary>
    /// Event raised when authentication state changes
    /// </summary>
    public event Action<bool>? AuthenticationChanged;

    /// <summary>
    /// Initializes a new instance of the AuthenticationService class
    /// </summary>
    public AuthenticationService(
        HttpClient httpClient,
        IOptions<CmsAdminOptions> options,
        ILogger<AuthenticationService> logger,
        IStateStorageService stateStorage,
        IGitHubTokenProvider tokenProvider,
        IGitHubApiClient apiClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _stateStorage = stateStorage ?? throw new ArgumentNullException(nameof(stateStorage));
        _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));

        // Get options from the injected CmsAdminOptions
        var options_value = options.Value;
        _githubOptions = options_value.GitHub;
        _authOptions = options_value.Authentication;

        // Try to restore auth state
        _ = RestoreAuthStateAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public bool IsAuthenticated => !string.IsNullOrEmpty(_accessToken);

    /// <inheritdoc/>
    public string? AccessToken => _accessToken;

    /// <inheritdoc/>
    public string? Username => _username;

    /// <inheritdoc/>
    public async Task<bool> AuthenticateWithGitHubAsync(string code)
    {
        try
        {
            // Check if credentials are configured
            if (!AreCredentialsConfigured())
                return false;

            // Exchange code for token
            _accessToken = await _tokenProvider.ExchangeCodeForTokenAsync(
                code,
                _authOptions.GitHubClientId,
                _authOptions.GitHubClientSecret);

            if (string.IsNullOrEmpty(_accessToken))
                return false;

            // Set the token on the API client immediately
            _apiClient.SetAccessToken(_accessToken);

            // Get user information
            var success = await GetUserInfoAsync();

            // Save state if successful
            if (success)
            {
                await PersistAuthStateAsync();
                AuthenticationChanged?.Invoke(true);
            }
            else
            {
                _accessToken = null;
            }

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authenticating with GitHub");
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> SetAccessTokenAsync(string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Attempted to set empty token");
                return false;
            }

            _accessToken = token;
            _logger.LogInformation("Setting access token. Token length: {Length}", token.Length);

            // Set the token on the API client immediately
            _apiClient.SetAccessToken(token);

            var success = await GetUserInfoAsync();

            // Save state if successful
            if (success)
            {
                await PersistAuthStateAsync();
                AuthenticationChanged?.Invoke(true);
                _logger.LogInformation("Token validated successfully");
            }
            else
            {
                _accessToken = null;
                await RemoveAuthStateAsync();
                _logger.LogWarning("Token validation failed");
            }

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating access token");
            _accessToken = null;
            await RemoveAuthStateAsync();
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task SignOutAsync()
    {
        _accessToken = null;
        _username = null;
        await RemoveAuthStateAsync();
        AuthenticationChanged?.Invoke(false);
    }

    /// <summary>
    /// Checks if client credentials are configured
    /// </summary>
    private bool AreCredentialsConfigured()
    {
        if (string.IsNullOrEmpty(_authOptions.GitHubClientId) ||
            string.IsNullOrEmpty(_authOptions.GitHubClientSecret))
        {
            _logger.LogError("GitHub client ID or secret is missing. Cannot authenticate.");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Gets user information using the current access token
    /// </summary>
    private async Task<bool> GetUserInfoAsync()
    {
        if (string.IsNullOrEmpty(_accessToken))
        {
            _logger.LogWarning("Cannot get user info - access token is empty");
            return false;
        }

        try
        {
            var request = CreateUserInfoRequest();
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("GitHub API returned non-success status code: {StatusCode}", response.StatusCode);
                return false;
            }

            response.EnsureSuccessStatusCode();

            var userInfo = await response.Content.ReadFromJsonAsync<GitHubUserInfo>();
            _username = userInfo?.Login;

            _logger.LogInformation("Retrieved user info: {Username}", _username);
            return !string.IsNullOrEmpty(_username);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user info from GitHub");
            _accessToken = null;
            _username = null;
            return false;
        }
    }

    /// <summary>
    /// Creates the HTTP request to get user information
    /// </summary>
    private HttpRequestMessage CreateUserInfoRequest()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user");

        // GitHub API prefers "token" scheme rather than "Bearer"
        request.Headers.Authorization = new AuthenticationHeaderValue("token", _accessToken);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.UserAgent.Add(new ProductInfoHeaderValue("OsirionBlogCMS", "2.0"));

        return request;
    }

    /// <summary>
    /// Persists authentication state to storage
    /// </summary>
    private async Task PersistAuthStateAsync()
    {
        if (!_stateStorage.IsInitialized)
        {
            await _stateStorage.InitializeAsync();
        }

        if (_stateStorage.IsInitialized)
        {
            await _stateStorage.SaveStateAsync("github_auth_token", _accessToken);
            await _stateStorage.SaveStateAsync("github_username", _username);
            _logger.LogDebug("Auth state persisted to storage. Token exists: {HasToken}", !string.IsNullOrEmpty(_accessToken));
        }
        else
        {
            _logger.LogWarning("Could not persist auth state - storage not initialized");
        }
    }

    /// <summary>
    /// Restores authentication state from storage
    /// </summary>
    private async Task RestoreAuthStateAsync()
    {
        if (!_stateStorage.IsInitialized)
        {
            await _stateStorage.InitializeAsync();
        }

        if (_stateStorage.IsInitialized)
        {
            _accessToken = await _stateStorage.GetStateAsync<string>("github_auth_token");
            _username = await _stateStorage.GetStateAsync<string>("github_username");

            _logger.LogDebug("Auth state restored from storage. Token exists: {HasToken}", !string.IsNullOrEmpty(_accessToken));

            // Validate restored token
            if (!string.IsNullOrEmpty(_accessToken))
            {
                // Set the token on the API client
                _apiClient.SetAccessToken(_accessToken);

                if (!await GetUserInfoAsync())
                {
                    _logger.LogWarning("Restored token validation failed");
                    await RemoveAuthStateAsync();
                }
                else
                {
                    _logger.LogInformation("Restored token validated successfully");
                }
            }
        }
        else
        {
            _logger.LogWarning("Could not restore auth state - storage not initialized");
        }
    }

    /// <summary>
    /// Removes authentication state from storage
    /// </summary>
    private async Task RemoveAuthStateAsync()
    {
        if (!_stateStorage.IsInitialized)
        {
            await _stateStorage.InitializeAsync();
        }

        if (_stateStorage.IsInitialized)
        {
            await _stateStorage.RemoveStateAsync("github_auth_token");
            await _stateStorage.RemoveStateAsync("github_username");
            _logger.LogDebug("Auth state removed from storage");
        }
        else
        {
            _logger.LogWarning("Could not remove auth state - storage not initialized");
        }
    }

    /// <summary>
    /// GitHub user information response model
    /// </summary>
    private class GitHubUserInfo
    {
        [JsonPropertyName("login")]
        public string? Login { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("avatar_url")]
        public string? AvatarUrl { get; set; }
    }
}