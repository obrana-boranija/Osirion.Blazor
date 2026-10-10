using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Options.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Infrastructure.Services;

/// <summary>
/// Service for GitHub authentication with repository adapter support
/// </summary>
/// <remarks>
/// <para>
/// The user's token stays inside this instance. The service never configures a GitHub API client, so the
/// token cannot reach a client used by the public site or by another user. Admin services in the same
/// scope read the token from <see cref="AccessToken"/> or receive it explicitly.
/// </para>
/// <para>
/// The token is held in memory only and never written to <see cref="IStateStorageService"/>, which keeps
/// values in plain text. The configured <see cref="AuthenticationOptions.PersonalAccessToken"/> never signs
/// anyone in here; admin services use it on the server when no user token is set.
/// </para>
/// </remarks>
public class AuthenticationService : IAuthenticationService
{
    /// <summary>
    /// The storage key under which earlier versions kept the token in plain text. It is only ever removed.
    /// </summary>
    private const string LegacyTokenStorageKey = "github_auth_token";

    private readonly HttpClient _httpClient;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly IStateStorageService _stateStorage;
    private readonly IGitHubTokenProvider _tokenProvider;
    private readonly GitHubOptions _githubOptions;
    private readonly AuthenticationOptions _authOptions;

    private string? _accessToken;
    private string? _username;
    private bool _initialized = false;
    private string _currentProviderName = string.Empty;

    /// <summary>
    /// Event raised when authentication state changes
    /// </summary>
    public event Action<bool>? AuthenticationChanged;

    /// <summary>
    /// Initializes a new instance of the AuthenticationService class
    /// </summary>
    /// <param name="httpClient">The HTTP client used to look up the GitHub user</param>
    /// <param name="options">The CMS admin options</param>
    /// <param name="logger">The logger instance</param>
    /// <param name="stateStorage">The storage for the authentication state</param>
    /// <param name="tokenProvider">The provider that exchanges OAuth codes for tokens</param>
    /// <param name="apiClientFactory">
    /// Kept for constructor compatibility. The service no longer takes a GitHub API client from the factory,
    /// because pushing a user's token onto a client is how it used to leak to other callers.
    /// </param>
    public AuthenticationService(
        HttpClient httpClient,
        IOptions<CmsAdminOptions> options,
        ILogger<AuthenticationService> logger,
        IStateStorageService stateStorage,
        IGitHubTokenProvider tokenProvider,
        IGitHubApiClientFactory apiClientFactory)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _stateStorage = stateStorage ?? throw new ArgumentNullException(nameof(stateStorage));
        _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
        ArgumentNullException.ThrowIfNull(apiClientFactory);

        // Get options from the injected CmsAdminOptions
        var options_value = options.Value;
        _githubOptions = options_value.GitHub;
        _authOptions = options_value.Authentication;
    }

    /// <summary>
    /// Sets the provider to use for authentication
    /// </summary>
    public void SetProvider(string providerName)
    {
        _currentProviderName = providerName;
    }

    /// <inheritdoc/>
    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(_accessToken);

    /// <inheritdoc/>
    public string? AccessToken => _accessToken;

    /// <inheritdoc/>
    public string? Username => _username;

    /// <summary>
    /// Initializes the authentication service
    /// </summary>
    /// <remarks>
    /// Removes any token an earlier version left in plain-text storage. It neither restores a session nor
    /// signs in with the configured personal access token.
    /// </remarks>
    public async Task InitializeAsync()
    {
        if (_initialized) return;

        _logger.LogInformation("Initializing authentication service");

        await RemoveLegacyTokenAsync();

        _initialized = true;
        _logger.LogInformation("Authentication service initialized");
    }

    /// <inheritdoc/>
    public async Task<bool> AuthenticateWithGitHubAsync(string code)
    {
        try
        {
            // Check if credentials are configured
            if (!AreCredentialsConfigured())
                return false;

            // Exchange code for token
            var token = await _tokenProvider.ExchangeCodeForTokenAsync(
                code,
                _authOptions.GitHubClientId,
                _authOptions.GitHubClientSecret);

            if (string.IsNullOrWhiteSpace(token))
                return false;

            // Get user information; a rejected token leaves the current sign-in unchanged
            var username = await GetUsernameAsync(token);
            if (username is null)
                return false;

            await CompleteSignInAsync(token, username);
            return true;
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
            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogWarning("Attempted to set empty token");
                return false;
            }

            _logger.LogInformation("Setting access token. Token length: {Length}", token.Length);

            // The token replaces the current one only once GitHub accepts it, so a rejected token never
            // leaves this service signed out while admin services of the same scope keep the previous token
            var username = await GetUsernameAsync(token);
            if (username is null)
            {
                _logger.LogWarning("Token validation failed");
                if (!IsAuthenticated)
                {
                    await RemoveAuthStateAsync();
                }

                return false;
            }

            await CompleteSignInAsync(token, username);
            _logger.LogInformation("Token validated successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating access token");
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
        if (string.IsNullOrWhiteSpace(_authOptions.GitHubClientId) ||
            string.IsNullOrWhiteSpace(_authOptions.GitHubClientSecret))
        {
            _logger.LogError("GitHub client ID or secret is missing. Cannot authenticate.");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Makes <paramref name="token"/> the signed-in token, after GitHub accepted it for <paramref name="username"/>
    /// </summary>
    private async Task CompleteSignInAsync(string token, string username)
    {
        await PersistAuthStateAsync(username);

        _accessToken = token;
        _username = username;
        AuthenticationChanged?.Invoke(true);
    }

    /// <summary>
    /// Gets the login of the GitHub user that <paramref name="token"/> belongs to, without changing this service
    /// </summary>
    /// <returns>The login, or <see langword="null"/> when GitHub does not accept the token</returns>
    private async Task<string?> GetUsernameAsync(string token)
    {
        try
        {
            using var request = CreateUserInfoRequest(token);
            using var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("GitHub API returned non-success status code: {StatusCode}", response.StatusCode);
                return null;
            }

            var userInfo = await response.Content.ReadFromJsonAsync<GitHubUserInfo>();
            var username = userInfo?.Login;

            _logger.LogInformation("Retrieved user info: {Username}", username);
            return string.IsNullOrWhiteSpace(username) ? null : username;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user info from GitHub");
            return null;
        }
    }

    /// <summary>
    /// Creates the HTTP request to get user information
    /// </summary>
    private static HttpRequestMessage CreateUserInfoRequest(string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user");

        // GitHub API prefers "token" scheme rather than "Bearer"
        request.Headers.Authorization = new AuthenticationHeaderValue("token", token);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.UserAgent.Add(new ProductInfoHeaderValue("OsirionBlogCMS", "2.0"));

        return request;
    }

    /// <summary>
    /// Persists authentication state to storage
    /// </summary>
    private async Task PersistAuthStateAsync(string username)
    {
        if (!_stateStorage.IsInitialized)
        {
            await _stateStorage.InitializeAsync();
        }

        if (_stateStorage.IsInitialized)
        {
            // The token itself is never persisted here: this storage keeps values in plain text
            await _stateStorage.RemoveStateAsync(LegacyTokenStorageKey);
            await _stateStorage.SaveStateAsync("github_username", username);
            // Also store auth_status and last_login_method for consistency with LoginViewModel
            await _stateStorage.SaveStateAsync("auth_status", true);
            await _stateStorage.SaveStateAsync("last_login_method", "pat");
            _logger.LogDebug("Auth state persisted to storage without the token");
        }
        else
        {
            _logger.LogWarning("Could not persist auth state - storage not initialized");
        }
    }

    /// <summary>
    /// Removes a token that an earlier version kept in plain-text storage
    /// </summary>
    private async Task RemoveLegacyTokenAsync()
    {
        if (!_stateStorage.IsInitialized)
        {
            await _stateStorage.InitializeAsync();
        }

        if (_stateStorage.IsInitialized)
        {
            await _stateStorage.RemoveStateAsync(LegacyTokenStorageKey);
        }
        else
        {
            _logger.LogWarning("Could not remove the legacy token because storage is not initialized");
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
            await _stateStorage.RemoveStateAsync(LegacyTokenStorageKey);
            await _stateStorage.RemoveStateAsync("github_username");
            await _stateStorage.RemoveStateAsync("auth_status");
            await _stateStorage.RemoveStateAsync("last_login_method");
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
