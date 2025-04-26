using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Core.Interfaces;
using Osirion.Blazor.Cms.Core.Providers.Interfaces;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Core.Services;

/// <summary>
/// Service for GitHub authentication
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly string? _clientId;
    private readonly string? _clientSecret;
    private string? _accessToken;
    private string? _username;
    private readonly IStateStorageService _stateStorage;
    private readonly IGitHubTokenProvider _tokenProvider;

    /// <summary>
    /// Initializes a new instance of the AuthenticationService class
    /// </summary>
    public AuthenticationService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<AuthenticationService> logger,
        IStateStorageService stateStorage,
        IGitHubTokenProvider tokenProvider)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _stateStorage = stateStorage ?? throw new ArgumentNullException(nameof(stateStorage));
        _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));

        // Read GitHub configuration
        _clientId = configuration["GitHub:ClientId"];
        _clientSecret = configuration["GitHub:ClientSecret"];
        _accessToken = configuration["GitHub:ApiToken"];

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
            _accessToken = await _tokenProvider.ExchangeCodeForTokenAsync(code, _clientId!, _clientSecret!);

            if (string.IsNullOrEmpty(_accessToken))
                return false;

            // Get user information
            var success = await GetUserInfoAsync();

            // Save state if successful
            if (success)
            {
                await PersistAuthStateAsync();
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
            _accessToken = token;
            var success = await GetUserInfoAsync();

            // Save state if successful
            if (success)
            {
                await PersistAuthStateAsync();
            }
            else
            {
                await RemoveAuthStateAsync();
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
    }

    /// <summary>
    /// Checks if client credentials are configured
    /// </summary>
    private bool AreCredentialsConfigured()
    {
        if (string.IsNullOrEmpty(_clientId) || string.IsNullOrEmpty(_clientSecret))
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
            return false;
        }

        try
        {
            var request = CreateUserInfoRequest();
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var userInfo = await response.Content.ReadFromJsonAsync<GitHubUserInfo>();
            _username = userInfo?.Login;
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
        if (_stateStorage.IsInitialized)
        {
            await _stateStorage.SaveStateAsync("github_auth_token", _accessToken);
            await _stateStorage.SaveStateAsync("github_username", _username);
        }
    }

    /// <summary>
    /// Restores authentication state from storage
    /// </summary>
    private async Task RestoreAuthStateAsync()
    {
        if (_stateStorage.IsInitialized)
        {
            _accessToken = await _stateStorage.GetStateAsync<string>("github_auth_token");
            _username = await _stateStorage.GetStateAsync<string>("github_username");

            // Validate restored token
            if (!string.IsNullOrEmpty(_accessToken))
            {
                if (!await GetUserInfoAsync())
                {
                    await RemoveAuthStateAsync();
                }
            }
        }
    }

    /// <summary>
    /// Removes authentication state from storage
    /// </summary>
    private async Task RemoveAuthStateAsync()
    {
        if (_stateStorage.IsInitialized)
        {
            await _stateStorage.RemoveStateAsync("github_auth_token");
            await _stateStorage.RemoveStateAsync("github_username");
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