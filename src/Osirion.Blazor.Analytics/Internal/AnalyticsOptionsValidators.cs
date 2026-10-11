using Microsoft.Extensions.Options;
using Osirion.Blazor.Analytics.Options;

namespace Osirion.Blazor.Analytics.Internal;

/// <summary>Validates <see cref="ClarityOptions"/> at startup. Disabled providers are not checked.</summary>
internal sealed class ClarityOptionsValidator : IValidateOptions<ClarityOptions>
{
    public ValidateOptionsResult Validate(string? name, ClarityOptions options)
    {
        if (!options.Enabled)
        {
            return ValidateOptionsResult.Success;
        }

        return string.IsNullOrWhiteSpace(options.TrackerUrl) || AnalyticsFormats.IsHttpOrProtocolRelativeUrl(options.TrackerUrl)
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail("Osirion:Analytics:Clarity:TrackerUrl must be an absolute http(s) URL.");
    }
}

/// <summary>Validates <see cref="MatomoOptions"/> at startup. Disabled providers are not checked.</summary>
internal sealed class MatomoOptionsValidator : IValidateOptions<MatomoOptions>
{
    public ValidateOptionsResult Validate(string? name, MatomoOptions options)
    {
        if (!options.Enabled || string.IsNullOrWhiteSpace(options.TrackerUrl))
        {
            return ValidateOptionsResult.Success;
        }

        return AnalyticsFormats.IsHttpOrProtocolRelativeUrl(options.TrackerUrl)
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail("Osirion:Analytics:Matomo:TrackerUrl must be an absolute http(s) URL or a protocol-relative URL such as //analytics.example.com/.");
    }
}

/// <summary>Validates <see cref="GA4Options"/> at startup. Disabled providers are not checked.</summary>
internal sealed class GA4OptionsValidator : IValidateOptions<GA4Options>
{
    public ValidateOptionsResult Validate(string? name, GA4Options options)
    {
        if (!options.Enabled || string.IsNullOrWhiteSpace(options.MeasurementId))
        {
            return ValidateOptionsResult.Success;
        }

        return AnalyticsFormats.IsGoogleTagId(options.MeasurementId)
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail("Osirion:Analytics:GA4:MeasurementId must look like G-XXXXXXXXXX.");
    }
}

/// <summary>Validates <see cref="YandexMetricaOptions"/> at startup. Disabled providers are not checked.</summary>
internal sealed class YandexMetricaOptionsValidator : IValidateOptions<YandexMetricaOptions>
{
    public ValidateOptionsResult Validate(string? name, YandexMetricaOptions options)
    {
        if (!options.Enabled)
        {
            return ValidateOptionsResult.Success;
        }

        var failures = new List<string>();

        if (!string.IsNullOrWhiteSpace(options.CounterId) && !AnalyticsFormats.IsYandexCounterId(options.CounterId))
        {
            failures.Add("Osirion:Analytics:YandexMetrica:CounterId must be a numeric counter ID.");
        }

        if (!string.IsNullOrWhiteSpace(options.AlternativeCdn) && !AnalyticsFormats.IsHttpUrl(options.AlternativeCdn))
        {
            failures.Add("Osirion:Analytics:YandexMetrica:AlternativeCdn must be an absolute http(s) URL.");
        }

        return failures.Count == 0 ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(failures);
    }
}

/// <summary>Validates <see cref="PlausibleOptions"/> at startup. Disabled providers are not checked.</summary>
internal sealed class PlausibleOptionsValidator : IValidateOptions<PlausibleOptions>
{
    public ValidateOptionsResult Validate(string? name, PlausibleOptions options)
    {
        if (!options.Enabled)
        {
            return ValidateOptionsResult.Success;
        }

        var failures = new List<string>();

        if (!string.IsNullOrWhiteSpace(options.ScriptUrl) && !AnalyticsFormats.IsHttpOrSiteRelativeUrl(options.ScriptUrl))
        {
            failures.Add("Osirion:Analytics:Plausible:ScriptUrl must be an http(s) URL or a path such as /js/script.js.");
        }

        if (!string.IsNullOrWhiteSpace(options.Endpoint) && !AnalyticsFormats.IsHttpOrSiteRelativeUrl(options.Endpoint))
        {
            failures.Add("Osirion:Analytics:Plausible:Endpoint must be an http(s) URL or a path such as /api/event.");
        }

        return failures.Count == 0 ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(failures);
    }
}

/// <summary>Validates <see cref="UmamiOptions"/> at startup. Disabled providers are not checked.</summary>
internal sealed class UmamiOptionsValidator : IValidateOptions<UmamiOptions>
{
    public ValidateOptionsResult Validate(string? name, UmamiOptions options)
    {
        if (!options.Enabled)
        {
            return ValidateOptionsResult.Success;
        }

        var failures = new List<string>();

        if (!string.IsNullOrWhiteSpace(options.SiteId) && !Guid.TryParse(options.SiteId, out _))
        {
            failures.Add("Osirion:Analytics:Umami:SiteId must be the website ID, a GUID.");
        }

        if (!AnalyticsFormats.IsHttpOrSiteRelativeUrl(options.ScriptUrl))
        {
            failures.Add("Osirion:Analytics:Umami:ScriptUrl must be an http(s) URL or a path such as /script.js.");
        }

        if (!string.IsNullOrWhiteSpace(options.HostUrl) && !AnalyticsFormats.IsHttpOrProtocolRelativeUrl(options.HostUrl))
        {
            failures.Add("Osirion:Analytics:Umami:HostUrl must be an absolute http(s) URL.");
        }

        return failures.Count == 0 ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(failures);
    }
}
