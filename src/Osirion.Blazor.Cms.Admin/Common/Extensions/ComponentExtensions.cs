using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;

namespace Osirion.Blazor.Cms.Admin.Common.Extensions;

public static class ComponentExtensions
{
    public static string GetCssClassNames(this ComponentBase component, string cssClass)
    {
        // Get the component's type name in kebab-case
        var componentName = ToKebabCase(component.GetType().Name.Replace("Base", ""));

        // Combine with provided CSS class if any
        return string.IsNullOrEmpty(cssClass)
            ? $"osirion-{componentName}"
            : $"osirion-{componentName} {cssClass}";
    }

    public static string GetFormCssClass(this ComponentBase component, string cssClass, bool isValid)
    {
        var baseClass = GetCssClassNames(component, cssClass);
        return isValid ? baseClass : $"{baseClass} osirion-form-invalid";
    }

    public static async Task HandleKeyPressAsync(this KeyboardEventArgs e, Func<Task> enterAction, Func<Task>? escapeAction = null)
    {
        if (e.Key.Equals("Enter", StringComparison.OrdinalIgnoreCase))
        {
            await enterAction();
        }
        else if (e.Key.Equals("Escape", StringComparison.OrdinalIgnoreCase) && escapeAction != null)
        {
            await escapeAction();
        }
    }

    public static string ToKebabCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        var result = new System.Text.StringBuilder();

        for (int i = 0; i < input.Length; i++)
        {
            if (char.IsUpper(input[i]) && i > 0)
            {
                result.Append('-');
            }

            result.Append(char.ToLowerInvariant(input[i]));
        }

        return result.ToString();
    }

    public static EditContext CreateEditContext<T>(this T model) where T : class
    {
        return new EditContext(model);
    }
}