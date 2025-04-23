using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Osirion.Blazor.Cms.Admin.Services;

/// <summary>
/// Service for GitHub authentication
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Gets whether the user is authenticated
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets the current access token if authenticated
    /// </summary>
    string? AccessToken { get; }

    /// <summary>
    /// Gets the username of the authenticated user
    /// </summary>
    string? Username { get; }

    /// <summary>
    /// Authenticates with GitHub using OAuth flow
    /// </summary>
    Task<bool> AuthenticateWithGitHubAsync(string code);

    /// <summary>
    /// Sets an access token directly
    /// </summary>
    Task<bool> SetAccessTokenAsync(string token);

    /// <summary>
    /// Signs out
    /// </summary>
    Task SignOutAsync();
}

/// <summary>
/// Implementation of IAuthenticationService for GitHub authentication
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly string? _clientId;
    private readonly string? _clientSecret;
    private string? _accessToken;
    private string? _username;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationService"/> class.
    /// </summary>
    public AuthenticationService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<AuthenticationService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Read GitHub configuration
        _clientId = configuration["GitHub:ClientId"];
        _clientSecret = configuration["GitHub:ClientSecret"];
        _accessToken = configuration["GitHub:ApiToken"];
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
            // If no client ID or secret, we can't authenticate
            if (string.IsNullOrEmpty(_clientId) || string.IsNullOrEmpty(_clientSecret))
            {
                _logger.LogError("GitHub client ID or secret is missing. Cannot authenticate.");
                return false;
            }

            // Exchange code for access token
            var tokenRequest = new
            {
                client_id = _clientId,
                client_secret = _clientSecret,
                code
            };

            var content = new StringContent(
                JsonSerializer.Serialize(tokenRequest),
                Encoding.UTF8
            );
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            // GitHub's token endpoint
            var response = await _httpClient.PostAsync(
                "https://github.com/login/oauth/access_token",
                content
            );

            response.EnsureSuccessStatusCode();

            // Parse response to get access token
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.Content.Headers.ContentType?.MediaType == "application/json")
            {
                var tokenResponse = JsonSerializer.Deserialize<GitHubTokenResponse>(responseContent);
                _accessToken = tokenResponse?.AccessToken;
            }
            else
            {
                // Parse as form url encoded
                var formValues = responseContent.Split('&')
                    .Select(v => v.Split('='))
                    .ToDictionary(pair => pair[0], pair => pair.Length > 1 ? pair[1] : string.Empty);

                _accessToken = formValues.ContainsKey("access_token") ? formValues["access_token"] : null;
            }

            if (string.IsNullOrEmpty(_accessToken))
            {
                _logger.LogError("Failed to get access token from GitHub");
                return false;
            }

            // Get user information
            await GetUserInfoAsync();
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
            _accessToken = token;
            return await GetUserInfoAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating access token");
            _accessToken = null;
            return false;
        }
    }

    /// <inheritdoc/>
    public Task SignOutAsync()
    {
        _accessToken = null;
        _username = null;
        return Task.CompletedTask;
    }

    private async Task<bool> GetUserInfoAsync()
    {
        if (string.IsNullOrEmpty(_accessToken))
        {
            return false;
        }

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user");
            request.Headers.Authorization = new AuthenticationHeaderValue("token", _accessToken);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.UserAgent.Add(new ProductInfoHeaderValue("OsirionBlogCMS", "2.0"));

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

    private class GitHubTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }

        [JsonPropertyName("scope")]
        public string? Scope { get; set; }
    }

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