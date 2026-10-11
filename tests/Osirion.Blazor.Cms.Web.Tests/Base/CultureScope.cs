using System.Globalization;

namespace Osirion.Blazor.Cms.Web.Tests.Base;

/// <summary>
/// Sets the current culture and UI culture until disposed, then restores the previous ones, so a test that
/// asserts formatted dates gives the same result on every machine.
/// </summary>
internal sealed class CultureScope : IDisposable
{
    private readonly CultureInfo _originalCulture = CultureInfo.CurrentCulture;
    private readonly CultureInfo _originalUiCulture = CultureInfo.CurrentUICulture;

    public CultureScope(string cultureName)
    {
        var culture = CultureInfo.GetCultureInfo(cultureName);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
    }

    public void Dispose()
    {
        CultureInfo.CurrentCulture = _originalCulture;
        CultureInfo.CurrentUICulture = _originalUiCulture;
    }
}
