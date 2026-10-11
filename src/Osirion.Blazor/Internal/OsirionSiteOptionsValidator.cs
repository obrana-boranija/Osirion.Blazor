using Microsoft.Extensions.Options;
using Osirion.Blazor.Options;

namespace Osirion.Blazor.Internal;

/// <summary>Fails startup with every problem of the site options, each naming its configuration key.</summary>
internal sealed class OsirionSiteOptionsValidator : IValidateOptions<OsirionSiteOptions>
{
    public ValidateOptionsResult Validate(string? name, OsirionSiteOptions options)
    {
        var errors = options.GetErrors();
        return errors.Count == 0 ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(errors);
    }
}
