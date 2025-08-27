using System.Collections.Generic;
using System.Threading.Tasks;
using Osirion.Blazor.Components;

namespace Osirion.Blazor.Theming.Services;

public interface ICssFrameworkDetector
{
    /// <summary>
    /// Detects the CSS framework being used in the application.
    /// </summary>
    /// <returns>The detected CSS framework.</returns>
    Task<CssFramework> DetectFrameworkAsync();

    /// <summary>
    /// Gets all detected frameworks.
    /// </summary>
    /// <returns>List of detected frameworks.</returns>
    Task<List<CssFramework>> GetDetectedFrameworksAsync();
}
