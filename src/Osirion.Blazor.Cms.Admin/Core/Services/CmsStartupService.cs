using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Core.State;

namespace Osirion.Blazor.Cms.Admin.Core.Services;

/// <summary>
/// Handles startup initialization for the CMS admin interface
/// </summary>
public class CmsStartupService
{
    private readonly StatePersistenceService _statePersistence;
    private readonly ILogger<CmsStartupService> _logger;
    private bool _isInitialized = false;

    public CmsStartupService(
        StatePersistenceService statePersistence,
        ILogger<CmsStartupService> logger)
    {
        _statePersistence = statePersistence;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        if (_isInitialized)
            return;

        try
        {
            _logger.LogInformation("Initializing CMS Admin module");

            // Initialize state
            await _statePersistence.InitializeAsync();

            _isInitialized = true;
            _logger.LogInformation("CMS Admin module initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing CMS Admin module");
        }
    }
}