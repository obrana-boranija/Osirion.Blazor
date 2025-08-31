using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Core.Models;
using Osirion.Blazor.Core.Services;

namespace Osirion.Blazor.Components;

/// <summary>
/// A comprehensive subscription card component with validation, accessibility, and framework-agnostic styling.
/// Supports newsletter signup, category preferences, and various layout options.
/// </summary>
public partial class OsirionSubscriptionCard : OsirionComponentBase
{
    #region Form Parameters

    /// <summary>
    /// Gets or sets the Id of subscription card.
    /// </summary>
    [Parameter, EditorRequired]
    public string Id { get; set; } = "subscription-card";

    /// <summary>
    /// Gets or sets the title displayed at the top of the subscription card.
    /// </summary>
    [Parameter]
    public string Title { get; set; } = "Stay Updated";

    /// <summary>
    /// Gets or sets the description text displayed below the title.
    /// </summary>
    [Parameter]
    public string Description { get; set; } = "Get the latest updates and insights delivered to your inbox.";

    /// <summary>
    /// Gets or sets whether to show the name field.
    /// </summary>
    [Parameter]
    public bool ShowNameField { get; set; } = false;

    /// <summary>
    /// Gets or sets the placeholder text for the name field.
    /// </summary>
    [Parameter]
    public string NamePlaceholder { get; set; } = "Your name (optional)";

    /// <summary>
    /// Gets or sets the placeholder text for the email field.
    /// </summary>
    [Parameter]
    public string EmailPlaceholder { get; set; } = "Enter your email address";

    /// <summary>
    /// Gets or sets the text for the subscribe button.
    /// </summary>
    [Parameter]
    public string SubscribeButtonText { get; set; } = "Subscribe";

    /// <summary>
    /// Gets or sets the text displayed on the subscribe button while submitting.
    /// </summary>
    [Parameter]
    public string SubscribingText { get; set; } = "Subscribing...";

    /// <summary>
    /// Gets or sets whether to show required field indicators (*).
    /// </summary>
    [Parameter]
    public bool ShowRequiredIndicators { get; set; } = true;

    #endregion

    #region Privacy and Category Parameters

    /// <summary>
    /// Gets or sets the privacy policy agreement text (supports HTML markup).
    /// If null or empty, the privacy policy checkbox will not be shown.
    /// </summary>
    [Parameter]
    public string? PrivacyPolicyText { get; set; } = "I agree to the <a href=\"/privacy-policy\" target=\"_blank\">privacy policy</a>";

    /// <summary>
    /// Gets or sets whether to show subscription categories for user selection.
    /// </summary>
    [Parameter]
    public bool ShowSubscriptionCategories { get; set; } = false;

    /// <summary>
    /// Gets or sets the available subscription categories.
    /// </summary>
    [Parameter]
    public IReadOnlyList<SubscriptionCategory>? SubscriptionCategories { get; set; }

    /// <summary>
    /// Gets or sets the text displayed above subscription categories.
    /// </summary>
    [Parameter]
    public string CategoriesText { get; set; } = "What would you like to receive?";

    /// <summary>
    /// Gets or sets whether to show subscription categories as a dropdown instead of inline checkboxes.
    /// </summary>
    [Parameter]
    public bool ShowCategoriesAsDropdown { get; set; } = false;

    /// <summary>
    /// Gets or sets the text for the categories dropdown button.
    /// </summary>
    [Parameter]
    public string CategoriesDropdownText { get; set; } = "Select Categories";

    #endregion

    #region Event Parameters

    /// <summary>
    /// Event callback invoked when the subscription form is submitted with valid data.
    /// </summary>
    [Parameter]
    public EventCallback<SubscriptionModel> OnSubscribe { get; set; }

    /// <summary>
    /// Event callback invoked when form validation fails.
    /// </summary>
    [Parameter]
    public EventCallback<EditContext> OnValidationFailed { get; set; }

    #endregion

    #region Success/Error Message Parameters

    /// <summary>
    /// Gets or sets the success message to display after successful subscription.
    /// </summary>
    [Parameter]
    public string SuccessMessage { get; set; } = "Thank you for subscribing! Please check your email to confirm your subscription.";

    /// <summary>
    /// Gets or sets the error message to display when subscription fails.
    /// </summary>
    [Parameter]
    public string ErrorMessage { get; set; } = "Sorry, there was an error processing your subscription. Please try again later.";

    /// <summary>
    /// Gets or sets the error message to display when spam is detected.
    /// </summary>
    [Parameter]
    public string SpamMessage { get; set; } = "Your submission appears to be spam and has been blocked.";

    /// <summary>
    /// Gets or sets the message to display when email is already subscribed.
    /// </summary>
    [Parameter]
    public string AlreadySubscribedMessage { get; set; } = "This email address is already subscribed to our newsletter.";

    /// <summary>
    /// Gets or sets whether to use the built-in email service (requires service registration)
    /// </summary>
    [Parameter]
    public bool UseBuiltInEmailService { get; set; } = false;

    #endregion

    #region Layout Parameters

    /// <summary>
    /// Gets or sets whether to use a compact layout (smaller form).
    /// </summary>
    [Parameter]
    public bool CompactLayout { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to use a dark theme styling.
    /// </summary>
    [Parameter]
    public bool UseDarkTheme { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to show the card border.
    /// </summary>
    [Parameter]
    public bool ShowBorder { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show the card shadow.
    /// </summary>
    [Parameter]
    public bool ShowShadow { get; set; } = true;

    [SupplyParameterFromForm]
    private SubscriptionModel FormModel { get; set; } = new();

    [SupplyParameterFromQuery]
    private int? SubmissionResultValue { get; set; }

    // For dropdown select multiple binding in Static SSR
    [SupplyParameterFromForm(Name = "SubscriptionCategories")]
    private string[]? SelectedCategoryIds { get; set; }

    #endregion

    #region Private Fields and Services

    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = default!;

    [Inject]
    private ILogger<OsirionSubscriptionCard> Logger { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    private SubscriptionSubmissionResult? SubmissionResult =>
        SubmissionResultValue.HasValue && Enum.IsDefined(typeof(SubscriptionSubmissionResult), SubmissionResultValue.Value)
            ? (SubscriptionSubmissionResult)SubmissionResultValue.Value
            : null;

    #endregion

    #region Lifecycle Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        // Initialize default subscription categories if showing categories but none provided
        if (ShowSubscriptionCategories && SubscriptionCategories == null)
        {
            SubscriptionCategories = new[]
            {
                new SubscriptionCategory
                {
                    Id = "newsletter",
                    Name = "Newsletter",
                    Description = "Weekly updates and featured content",
                    IsDefault = true
                },
                new SubscriptionCategory
                {
                    Id = "updates",
                    Name = "Product Updates",
                    Description = "New features and improvements",
                    IsDefault = true
                },
                new SubscriptionCategory
                {
                    Id = "tips",
                    Name = "Tips & Tutorials",
                    Description = "Best practices and how-to guides",
                    IsDefault = false
                }
            };
        }

        // Set default selected categories based on defaults
        if (ShowSubscriptionCategories && SubscriptionCategories != null)
        {
            FormModel.SubscriptionCategories = SubscriptionCategories
                .Where(c => c.IsDefault || c.IsRequired)
                .Select(c => c.Id)
                .ToList();
        }
    }

    #endregion

    #region Event Handlers

    private async Task HandleValidSubmit()
    {
        var subscriptionSubmissionResult = SubscriptionSubmissionResult.SpamDetected;

        // Check for spam (honeypot field should be empty)
        if (string.IsNullOrWhiteSpace(FormModel.Website))
        {
            try
            {
                // Process category selections from form submission
                ProcessCategorySelections();

                // Use built-in email service if enabled and registered
                if (UseBuiltInEmailService)
                {
                    var emailServiceFactory = ServiceProvider.GetService<EmailServiceFactory>();
                    if (emailServiceFactory != null)
                    {
                        var emailService = emailServiceFactory.CreateEmailService();
                        // For subscription, we'd need a different email service method
                        // This is a placeholder for now
                        Logger.LogInformation("Subscription submitted for {Email}", FormModel.Email);
                        subscriptionSubmissionResult = SubscriptionSubmissionResult.Success;
                    }
                    else
                    {
                        Logger.LogWarning("Email service factory not registered but UseBuiltInEmailService is true");
                    }
                }
                else
                {
                    // Invoke the subscribe callback for custom handling
                    await OnSubscribe.InvokeAsync(FormModel);

                    // If no explicit navigation happened, assume success
                    subscriptionSubmissionResult = SubscriptionSubmissionResult.Success;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error processing subscription submission");
                subscriptionSubmissionResult = SubscriptionSubmissionResult.ServerError;
            }
        }

        NavigateWithResult(subscriptionSubmissionResult);
    }

    private void ProcessCategorySelections()
    {
        if(!ShowSubscriptionCategories)
        {
            return;
        }

        // In Static SSR, process the selected categories from the form submission
        FormModel.SubscriptionCategories.Clear();
        
        // Add categories from checkbox selections (both dropdown and inline versions use same name)
        if (SelectedCategoryIds is not null)
        {
            foreach (var categoryId in SelectedCategoryIds)
            {
                if (!string.IsNullOrWhiteSpace(categoryId))
                {
                    FormModel.SubscriptionCategories.Add(categoryId);
                }
            }
        }
        
        // Always ensure required categories are included
        if (SubscriptionCategories is not null)
        {
            foreach (var category in SubscriptionCategories.Where(c => c.IsRequired))
            {
                if (!FormModel.SubscriptionCategories.Contains(category.Id))
                {
                    FormModel.SubscriptionCategories.Add(category.Id);
                }
            }
        }
    }

    private void NavigateWithResult(SubscriptionSubmissionResult result)
    {
        var uri = NavigationManager.GetUriWithQueryParameter("SubmissionResultValue", (int)result);

        // Note: In Static SSR, we cannot prevent EnhancedNavigation from scrolling to top
        // Users should configure EnhancedNavigation with ResetScrollOnNavigation=false
        // or disable it entirely when using subscription forms.
        // This has to be fixed in future versions of EnhancedNavigation.
        NavigationManager.NavigateTo(uri, forceLoad: false);
    }

    #endregion

    #region Helper Methods

    private bool IsCategorySelected(string categoryId)
    {
        return FormModel.SubscriptionCategories.Contains(categoryId);
    }

    private bool IsCategoryRequired(string categoryId)
    {
        return SubscriptionCategories?.FirstOrDefault(c => c.Id == categoryId)?.IsRequired == true;
    }

    #endregion

    #region CSS Class Methods

    private string GetSubscriptionCardContainerClass()
    {
        var classes = new List<string> { "osirion-subscription-card" };

        if (CompactLayout)
            classes.Add("osirion-subscription-card-compact");

        if (UseDarkTheme)
            classes.Add("osirion-subscription-card-dark");

        if (ShowBorder)
            classes.Add("osirion-subscription-card-bordered");

        if (ShowShadow)
            classes.Add("osirion-subscription-card-shadow");

        return CombineCssClasses(string.Join(" ", classes));
    }

    private string GetStatusMessageClass()
    {
        var alertVariant = SubmissionResult switch
        {
            SubscriptionSubmissionResult.Success => Severity.Success,
            SubscriptionSubmissionResult.ValidationError or SubscriptionSubmissionResult.ServerError => Severity.Error,
            SubscriptionSubmissionResult.SpamDetected => Severity.Warning,
            SubscriptionSubmissionResult.AlreadySubscribed => Severity.Info,
            _ => Severity.Info
        };

        return GetAlertClass(alertVariant);
    }

    private string GetStatusMessage()
    {
        return SubmissionResult switch
        {
            SubscriptionSubmissionResult.Success => SuccessMessage,
            SubscriptionSubmissionResult.ValidationError => "Please correct the errors below and try again.",
            SubscriptionSubmissionResult.ServerError => ErrorMessage,
            SubscriptionSubmissionResult.SpamDetected => SpamMessage,
            SubscriptionSubmissionResult.AlreadySubscribed => AlreadySubscribedMessage,
            _ => string.Empty
        };
    }

    private RenderFragment GetStatusIcon()
    {
        return SubmissionResult switch
        {
            SubscriptionSubmissionResult.Success => CreateSuccessIcon(),
            SubscriptionSubmissionResult.ValidationError => CreateErrorIcon(),
            SubscriptionSubmissionResult.ServerError => CreateErrorIcon(),
            SubscriptionSubmissionResult.SpamDetected => CreateWarningIcon(),
            SubscriptionSubmissionResult.AlreadySubscribed => CreateInfoIcon(),
            _ => CreateInfoIcon()
        };
    }

    private RenderFragment CreateSuccessIcon() => builder =>
    {
        builder.OpenElement(0, "svg");
        builder.AddAttribute(1, "xmlns", "http://www.w3.org/2000/svg");
        builder.AddAttribute(2, "width", "24");
        builder.AddAttribute(3, "height", "24");
        builder.AddAttribute(4, "fill", "currentColor");
        builder.AddAttribute(5, "class", "osirion-status-icon bi bi-check-circle");
        builder.AddAttribute(6, "viewBox", "0 0 20 20");
        builder.AddAttribute(7, "aria-hidden", "true");
        builder.OpenElement(8, "path");
        builder.AddAttribute(9, "d", "M8 15A7 7 0 1 1 8 1a7 7 0 0 1 0 14m0 1A8 8 0 1 0 8 0a8 8 0 0 0 0 16");
        builder.CloseElement();
        builder.OpenElement(10, "path");
        builder.AddAttribute(11, "d", "m10.97 4.97-.02.022-3.473 4.425-2.093-2.094a.75.75 0 0 0-1.06 1.06L6.97 11.03a.75.75 0 0 0 1.079-.02l3.992-4.99a.75.75 0 0 0-1.071-1.05");
        builder.CloseElement();
        builder.CloseElement();
    };

    private RenderFragment CreateErrorIcon() => builder =>
    {
        builder.OpenElement(0, "svg");
        builder.AddAttribute(1, "xmlns", "http://www.w3.org/2000/svg");
        builder.AddAttribute(2, "width", "24");
        builder.AddAttribute(3, "height", "24");
        builder.AddAttribute(4, "fill", "currentColor");
        builder.AddAttribute(5, "class", "osirion-status-icon bi bi-exclamation-circle");
        builder.AddAttribute(6, "viewBox", "0 0 20 20");
        builder.AddAttribute(7, "aria-hidden", "true");
        builder.OpenElement(8, "path");
        builder.AddAttribute(9, "d", "M8 15A7 7 0 1 1 8 1a7 7 0 0 1 0 14m0 1A8 8 0 1 0 8 0a8 8 0 0 0 0 16");
        builder.CloseElement();
        builder.OpenElement(10, "path");
        builder.AddAttribute(11, "d", "M7.002 11a1 1 0 1 1 2 0 1 1 0 0 1-2 0M7.1 4.995a.905.905 0 1 1 1.8 0l-.35 3.507a.552.552 0 0 1-1.1 0z");
        builder.CloseElement();
        builder.CloseElement();
    };

    private RenderFragment CreateWarningIcon() => builder =>
    {
        builder.OpenElement(0, "svg");
        builder.AddAttribute(1, "xmlns", "http://www.w3.org/2000/svg");
        builder.AddAttribute(2, "width", "24");
        builder.AddAttribute(3, "height", "24");
        builder.AddAttribute(4, "fill", "currentColor");
        builder.AddAttribute(5, "class", "osirion-status-icon bi bi-exclamation-triangle");
        builder.AddAttribute(6, "viewBox", "0 0 20 20");
        builder.AddAttribute(7, "aria-hidden", "true");
        builder.OpenElement(8, "path");
        builder.AddAttribute(9, "d", "M7.938 2.016A.13.13 0 0 1 8.002 2a.13.13 0 0 1 .063.016.15.15 0 0 1 .054.057l6.857 11.667c.036.06.035.124.002.183a.2.2 0 0 1-.054.06.1.1 0 0 1-.066.017H1.146a.1.1 0 0 1-.066-.017.2.2 0 0 1-.054-.06.18.18 0 0 1 .002-.183L7.884 2.073a.15.15 0 0 1 .054-.057m1.044-.45a1.13 1.13 0 0 0-1.96 0L.165 13.233c-.457.778.091 1.767.98 1.767h13.713c.889 0 1.438-.99.98-1.767z");
        builder.CloseElement();
        builder.OpenElement(10, "path");
        builder.AddAttribute(11, "d", "M7.002 12a1 1 0 1 1 2 0 1 1 0 0 1-2 0M7.1 5.995a.905.905 0 1 1 1.8 0l-.35 3.507a.552.552 0 0 1-1.1 0z");
        builder.CloseElement();
        builder.CloseElement();
    };

    private RenderFragment CreateInfoIcon() => builder =>
    {
        builder.OpenElement(0, "svg");
        builder.AddAttribute(1, "xmlns", "http://www.w3.org/2000/svg");
        builder.AddAttribute(2, "width", "24");
        builder.AddAttribute(3, "height", "24");
        builder.AddAttribute(4, "fill", "currentColor");
        builder.AddAttribute(5, "class", "osirion-status-icon bi bi-info-circle");
        builder.AddAttribute(6, "viewBox", "0 0 20 20");
        builder.AddAttribute(7, "aria-hidden", "true");
        builder.OpenElement(8, "path");
        builder.AddAttribute(9, "d", "M8 15A7 7 0 1 1 8 1a7 7 0 0 1 0 14m0 1A8 8 0 1 0 8 0a8 8 0 0 0 0 16");
        builder.CloseElement();
        builder.OpenElement(10, "path");
        builder.AddAttribute(11, "d", "m8.93 6.588-2.29.287-.082.38.45.083c.294.07.352.176.288.469l-.738 3.468c-.194.897.105 1.319.808 1.319.545 0 1.178-.252 1.465-.598l.088-.416c-.2.176-.492.246-.686.246-.275 0-.375-.193-.304-.533zM9 4.5a1 1 0 1 1-2 0 1 1 0 0 1 2 0");
        builder.CloseElement();
        builder.CloseElement();
    };

    #endregion
}
