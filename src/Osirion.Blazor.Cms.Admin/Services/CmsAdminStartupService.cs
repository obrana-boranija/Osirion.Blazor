using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Admin.Services.State;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options.Configuration;

namespace Osirion.Blazor.Cms.Admin.Services;

/// <summary>
/// Startup service for the CMS admin interface
/// </summary>
public class CmsAdminStartupService
{
    private readonly StateManager _stateManager;
    private readonly ErrorHandlingService _errorHandler;
    private readonly ILogger<CmsAdminStartupService> _logger;
    private readonly CmsAdminOptions _options;
    private readonly IAuthenticationService _authenticationService;
    private bool _isInitialized = false;

    public CmsAdminStartupService(
        StateManager stateManager,
        ErrorHandlingService errorHandler,
        IAuthenticationService authenticationService,
        IOptions<CmsAdminOptions> options,
        ILogger<CmsAdminStartupService> logger)
    {
        _stateManager = stateManager;
        _errorHandler = errorHandler;
        _options = options.Value;
        _logger = logger;
        _authenticationService = authenticationService;
    }

    /// <summary>
    /// Initializes the CMS admin module
    /// </summary>
    public async Task InitializeAsync()
    {
        if (_isInitialized)
            return;

        try
        {
            _logger.LogInformation("Initializing CMS Admin module");

            // Initialize authentication first
            await _authenticationService.InitializeAsync();

            // Then initialize state
            await _stateManager.InitializeAsync();

            _isInitialized = true;
            _logger.LogInformation("CMS Admin module initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing CMS Admin module");
            _errorHandler.HandleException(ex, "startup");
        }
    }
}