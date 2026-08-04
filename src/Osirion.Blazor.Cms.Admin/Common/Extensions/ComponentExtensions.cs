using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;

namespace Osirion.Blazor.Cms.Admin.Common.Extensions;

    /// <summary>Defines the public member type.</summary>
public static class ComponentExtensions
{
    /// <summary>Gets or sets the GetCssClassNames value.</summary>
    public static string GetCssClassNames(this ComponentBase component, string cssClass)
    {
        // Get the component's type name in kebab-case
        var componentName = ToKebabCase(component.GetType().Name.Replace("Base", ""));

        // Combine with provided CSS class if any
        return string.IsNullOrWhiteSpace(cssClass)
            ? $"osirion-{componentName}"
            : $"osirion-{componentName} {cssClass}";
    }

    /// <summary>Gets or sets the GetFormCssClass value.</summary>
    public static string GetFormCssClass(this ComponentBase component, string cssClass, bool isValid)
    {
        var baseClass = GetCssClassNames(component, cssClass);
        return isValid ? baseClass : $"{baseClass} osirion-form-invalid";
    }

    /// <summary>Performs the HandleKeyPress operation asynchronously.</summary>
    public static async Task HandleKeyPressAsync(this KeyboardEventArgs e, Func<Task> enterAction, Func<Task>? escapeAction = null)
    {
        if (e.Key.Equals("Enter", StringComparison.OrdinalIgnoreCase))
        {
            await enterAction();
        }
        else if (e.Key.Equals("Escape", StringComparison.OrdinalIgnoreCase) && escapeAction is not null)
        {
            await escapeAction();
        }
    }

    /// <summary>Gets or sets the ToKebabCase value.</summary>
    public static string ToKebabCase(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;

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

    /// <summary>Performs the public member operation.</summary>
    public static EditContext CreateEditContext<T>(this T model) where T : class
    {
        return new EditContext(model);
    }
}
