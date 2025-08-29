using Microsoft.AspNetCore.Components;
using System.ComponentModel;

namespace Osirion.Blazor.Example.Bootstrap.Components.Generic;
public partial class EnumSelect<TEnum> where TEnum : struct, Enum
{
    [Parameter] public string Label { get; set; } = "Select option";
    [Parameter] public string Placeholder { get; set; } = "Choose...";
    [Parameter] public bool IncludeEmptyOption { get; set; } = false;

    private static string GetDescription(TEnum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?
            .GetCustomAttributes(typeof(DescriptionAttribute), false)
            .Cast<DescriptionAttribute>()
            .FirstOrDefault();

        return attribute?.Description ?? value.ToString() ?? string.Empty;
    }

    protected override bool TryParseValueFromString(string value, out TEnum result, out string validationErrorMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result = default!;
            validationErrorMessage = null!;
            return true;
        }

        if (Enum.TryParse<TEnum>(value, out var parsed))
        {
            result = parsed;
            validationErrorMessage = null!;
            return true;
        }

        result = default!;
        validationErrorMessage = $"The selected value '{value}' is not valid for {typeof(TEnum).Name}.";
        return false;
    }

    private string CurrentValueAsString
    {
        get => CurrentValue.ToString() ?? string.Empty;
        set
        {
            if (TryParseValueFromString(value, out var parsedValue, out var error))
            {
                CurrentValue = parsedValue;
            }
        }
    }
}
