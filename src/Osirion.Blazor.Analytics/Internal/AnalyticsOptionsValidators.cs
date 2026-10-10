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
