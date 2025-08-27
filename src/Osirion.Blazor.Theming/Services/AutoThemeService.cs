using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Components;
using System.Threading.Tasks;

namespace Osirion.Blazor.Theming.Services;

/// <summary>
/// Theme service with automatic CSS framework detection.
/// </summary>
public class AutoThemeService : ThemeService
{
    private readonly ICssFrameworkDetector _detector;
    private CssFramework? _detected;
    private bool _detectionAttempted;

    public AutoThemeService(
        IOptions<ThemingOptions> options,
        ICssFrameworkDetector detector,
        IHttpContextAccessor? httpContextAccessor = null,
        ILogger<ThemeService>? logger = null)
        : base(options, httpContextAccessor, logger)
    {
        _detector = detector;
    }

    public override CssFramework CurrentFramework
    {
        get
        {
            if (base.CurrentFramework == CssFramework.None)
            {
                if (!_detectionAttempted)
                {
                    // Synchronously block for detection (safe in SSR)
                    _detected = _detector.DetectFrameworkAsync().GetAwaiter().GetResult();
                    _detectionAttempted = true;
                }
                return _detected ?? CssFramework.None;
            }
            return base.CurrentFramework;
        }
    }
}
