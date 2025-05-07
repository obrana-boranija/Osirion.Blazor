using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Linq.Expressions;

namespace Osirion.Blazor.Cms.Admin.Common.Base;

public abstract class ValidationComponentBase : EditableComponentBase
{
    [Parameter]
    public EditContext? EditContext { get; set; }

    [Parameter]
    public bool ValidateOnInit { get; set; } = false;

    protected bool IsValid { get; private set; } = true;
    protected Dictionary<string, List<string>> ValidationErrors { get; private set; } = new();

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (EditContext != null && ValidateOnInit)
        {
            ValidateModel();
        }
    }

    protected bool ValidateModel()
    {
        if (EditContext == null)
            return true;

        ValidationErrors.Clear();
        var isValid = EditContext.Validate();

        if (!isValid)
        {
            foreach (var validationMessage in EditContext.GetValidationMessages())
            {
                var field = validationMessage.ToString().Split(':').FirstOrDefault() ?? "General";

                if (!ValidationErrors.ContainsKey(field))
                {
                    ValidationErrors[field] = new List<string>();
                }

                ValidationErrors[field].Add(validationMessage.ToString());
            }
        }

        IsValid = isValid;
        return isValid;
    }

    protected string GetFieldValidationClass(Expression<Func<object?>> field)
    {
        if (EditContext == null)
            return string.Empty;

        var fieldIdentifier = FieldIdentifier.Create(field);
        var isValid = !EditContext.GetValidationMessages(fieldIdentifier).Any();

        return isValid ? "osirion-field-valid" : "osirion-field-invalid";
    }

    protected IEnumerable<string> GetFieldValidationMessages(Expression<Func<object?>> field)
    {
        if (EditContext == null)
            return Array.Empty<string>();

        var fieldIdentifier = FieldIdentifier.Create(field);
        return EditContext.GetValidationMessages(fieldIdentifier);
    }
}