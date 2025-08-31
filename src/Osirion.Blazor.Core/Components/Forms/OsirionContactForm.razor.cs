using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Core.Models;
using Osirion.Blazor.Core.Services;

namespace Osirion.Blazor.Components;

/// <summary>
/// A comprehensive contact form component with validation, accessibility, and framework-agnostic styling.
/// Includes optional contact information display section and follows Osirion design principles.
/// </summary>
public partial class OsirionContactForm : OsirionComponentBase
{
    #region Form Parameters

    /// <summary>
    /// Gets or sets the Id of contact form.
    /// </summary>
    [Parameter, EditorRequired]
    public string Id { get; set; } = "contact-form";

    /// <summary>
    /// Gets or sets the title displayed above the contact form.
    /// </summary>
    [Parameter]
    public string FormTitle { get; set; } = "Write us";

    /// <summary>
    /// Gets or sets the description text displayed below the form title.
    /// </summary>
    [Parameter]
    public string FormDescription { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the placeholder text for the name field.
    /// </summary>
    [Parameter]
    public string NamePlaceholder { get; set; } = "Your name";

    /// <summary>
    /// Gets or sets the placeholder text for the email field.
    /// </summary>
    [Parameter]
    public string EmailPlaceholder { get; set; } = "Your email address";

    /// <summary>
    /// Gets or sets the placeholder text for the subject field.
    /// </summary>
    [Parameter]
    public string SubjectPlaceholder { get; set; } = "Subject";

    /// <summary>
    /// Gets or sets the placeholder text for the message field.
    /// </summary>
    [Parameter]
    public string MessagePlaceholder { get; set; } = "Your message";

    /// <summary>
    /// Gets or sets the number of rows for the message textarea.
    /// </summary>
    [Parameter]
    public int MessageRows { get; set; } = 6;

    /// <summary>
    /// Gets or sets the text for the submit button.
    /// </summary>
    [Parameter]
    public string SubmitButtonText { get; set; } = "Send Message";

    /// <summary>
    /// Gets or sets the text displayed on the submit button while submitting.
    /// </summary>
    [Parameter]
    public string SubmittingText { get; set; } = "Sending...";

    /// <summary>
    /// Gets or sets whether to show the reset button.
    /// </summary>
    [Parameter]
    public bool ShowResetButton { get; set; } = false;

    /// <summary>
    /// Gets or sets the text for the reset button.
    /// </summary>
    [Parameter]
    public string ResetButtonText { get; set; } = "Reset";

    /// <summary>
    /// Gets or sets whether to show required field indicators (*).
    /// </summary>
    [Parameter]
    public bool ShowRequiredIndicators { get; set; } = true;

    #endregion

    #region Privacy and Subscription Parameters

    /// <summary>
    /// Gets or sets the privacy policy agreement text (supports HTML markup).
    /// If null or empty, the privacy policy checkbox will not be shown.
    /// </summary>
    [Parameter]
    public string? PrivacyPolicyText { get; set; } = "I agree to the <a href=\"/privacy-policy\" target=\"_blank\">privacy policy</a>";

    /// <summary>
    /// Gets or sets whether to show the subscription option checkbox.
    /// </summary>
    [Parameter]
    public bool ShowSubscribeOption { get; set; } = true;

    /// <summary>
    /// Gets or sets the text for the subscription checkbox.
    /// </summary>
    [Parameter]
    public string SubscribeText { get; set; } = "I would like to receive updates and newsletters";

    #endregion

    #region Contact Information Parameters

    /// <summary>
    /// Gets or sets whether to show the contact information section.
    /// </summary>
    [Parameter]
    public bool ShowContactInformation { get; set; } = true;

    /// <summary>
    /// Gets or sets the title for the contact information section.
    /// </summary>
    [Parameter]
    public string ContactInfoTitle { get; set; } = "Contact information";

    /// <summary>
    /// Gets or sets the contact information to display.
    /// </summary>
    [Parameter]
    public ContactInformation? ContactInfo { get; set; }

    #endregion

    #region Event Parameters

    /// <summary>
    /// Event callback invoked when the form is submitted with valid data.
    /// </summary>
    [Parameter]
    public EventCallback<ContactFormModel> OnSubmit { get; set; }

    /// <summary>
    /// Event callback invoked when the form is reset.
    /// </summary>
    [Parameter]
    public EventCallback OnReset { get; set; }

    /// <summary>
    /// Event callback invoked when form validation fails.
    /// </summary>
    [Parameter]
    public EventCallback<EditContext> OnValidationFailed { get; set; }

    #endregion

    #region Success/Error Message Parameters

    /// <summary>
    /// Gets or sets the success message to display after successful submission.
    /// </summary>
    [Parameter]
    public string SuccessMessage { get; set; } = "Thank you! Your message has been sent successfully. We'll get back to you soon.";

    /// <summary>
    /// Gets or sets the error message to display when submission fails.
    /// </summary>
    [Parameter]
    public string ErrorMessage { get; set; } = "Sorry, there was an error sending your message. Please try again later.";

    /// <summary>
    /// Gets or sets the error message to display when spam is detected.
    /// </summary>
    [Parameter]
    public string SpamMessage { get; set; } = "Your submission appears to be spam and has been blocked.";

    /// <summary>
    /// Gets or sets whether to use the built-in email service (requires service registration)
    /// </summary>
    [Parameter]
    public bool UseBuiltInEmailService { get; set; } = false;

    #endregion

    #region Layout Parameters

    /// <summary>
    /// Gets or sets whether to show the form and contact info side by side (true) or stacked (false).
    /// </summary>
    [Parameter]
    public bool SideBySideLayout { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to use a dark theme styling.
    /// </summary>
    [Parameter]
    public bool UseDarkTheme { get; set; } = false;

    [SupplyParameterFromForm]
    private ContactFormModel FormModel { get; set; } = new();

    [SupplyParameterFromQuery]
    private int? SubmissionResultValue { get; set; }

    #endregion

    #region Private Fields and Services

    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = default!;

    [Inject]
    private ILogger<OsirionContactForm> Logger { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    private ContactFormSubmissionResult? SubmissionResult =>
        SubmissionResultValue.HasValue && Enum.IsDefined(typeof(ContactFormSubmissionResult), SubmissionResultValue.Value)
            ? (ContactFormSubmissionResult)SubmissionResultValue.Value
            : null;

    #endregion

    #region Lifecycle Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        // Set default contact information if not provided
        ContactInfo ??= new ContactInformation
        {
            Address = "198 West 21th Street, Suite 721 New York NY 10016",
            Phone = "+ 1235 2355 98",
            Email = "info@yoursite.com",
            Website = "yoursite.com",
            Message = "We're open for any suggestion or just to have a chat"
        };
    }

    #endregion

    #region Event Handlers

    private async Task HandleValidSubmit()
    {
        var contactFormSubmissionResult = ContactFormSubmissionResult.SpamDetected;

        // Check for spam (honeypot field should be empty)
        if (string.IsNullOrWhiteSpace(FormModel.Website))
        {
            try
            {
                // Use built-in email service if enabled and registered
                if (UseBuiltInEmailService)
                {
                    var emailServiceFactory = ServiceProvider.GetService<EmailServiceFactory>();
                    if (emailServiceFactory != null)
                    {
                        var emailService = emailServiceFactory.CreateEmailService();
                        var result = await emailService.SendEmailAsync(FormModel);

                        if (result.IsSuccess)
                        {
                            await OnSubmit.InvokeAsync(FormModel);
                            contactFormSubmissionResult = ContactFormSubmissionResult.Success;
                        }
                        else
                        {
                            Logger.LogError("Email sending failed: {ErrorMessage}", result.ErrorMessage);
                            contactFormSubmissionResult = ContactFormSubmissionResult.ServerError;
                        }
                    }
                    else
                    {
                        Logger.LogWarning("Email service factory not registered but UseBuiltInEmailService is true");
                    }
                }
                else
                {
                    // Invoke the submit callback for custom handling
                    await OnSubmit.InvokeAsync(FormModel);

                    // If no explicit navigation happened, assume success
                    contactFormSubmissionResult = ContactFormSubmissionResult.Success;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error processing contact form submission");
                contactFormSubmissionResult = ContactFormSubmissionResult.ServerError;
            }
        }

        NavigateWithResult(contactFormSubmissionResult);
    }

    private void NavigateWithResult(ContactFormSubmissionResult result)
    {
        var uri = NavigationManager.GetUriWithQueryParameter("SubmissionResultValue", (int)result);
        NavigationManager.NavigateTo(uri, forceLoad: true);
    }

    #endregion

    #region CSS Class Methods

    private string GetContactFormContainerClass()
    {
        var classes = new List<string> { "osirion-contact-form-container" };

        if (SideBySideLayout)
            classes.Add("osirion-contact-form-side-by-side");
        else
            classes.Add("osirion-contact-form-stacked");

        if (UseDarkTheme)
            classes.Add("osirion-contact-form-dark");

        if (ShowContactInformation && ContactInfo != null)
            classes.Add("osirion-contact-form-with-info");

        return CombineCssClasses(string.Join(" ", classes));
    }

    private string GetStatusMessageClass()
    {
        var alertVariant = SubmissionResult switch
        {
            ContactFormSubmissionResult.Success => Severity.Success,
            ContactFormSubmissionResult.ValidationError or ContactFormSubmissionResult.ServerError => Severity.Error,
            ContactFormSubmissionResult.SpamDetected => Severity.Warning,
            _ => Severity.Info
        };

        return GetAlertClass(alertVariant);
    }

    private string GetStatusMessage()
    {
        return SubmissionResult switch
        {
            ContactFormSubmissionResult.Success => SuccessMessage,
            ContactFormSubmissionResult.ValidationError => "Please correct the errors below and try again.",
            ContactFormSubmissionResult.ServerError => ErrorMessage,
            ContactFormSubmissionResult.SpamDetected => SpamMessage,
            _ => string.Empty
        };
    }

    private RenderFragment GetStatusIcon()
    {
        return SubmissionResult switch
        {
            ContactFormSubmissionResult.Success => CreateSuccessIcon(),
            ContactFormSubmissionResult.ValidationError => CreateErrorIcon(),
            ContactFormSubmissionResult.ServerError => CreateErrorIcon(),
            ContactFormSubmissionResult.SpamDetected => CreateWarningIcon(),
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
        builder.CloseElement(); // Close path
        builder.OpenElement(10, "path");
        builder.AddAttribute(11, "d", "m10.97 4.97-.02.022-3.473 4.425-2.093-2.094a.75.75 0 0 0-1.06 1.06L6.97 11.03a.75.75 0 0 0 1.079-.02l3.992-4.99a.75.75 0 0 0-1.071-1.05");
        builder.CloseElement(); // Close path
        builder.CloseElement(); // Close svg
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
        builder.CloseElement(); // Close path
        builder.OpenElement(10, "path");
        builder.AddAttribute(11, "d", "M7.002 11a1 1 0 1 1 2 0 1 1 0 0 1-2 0M7.1 4.995a.905.905 0 1 1 1.8 0l-.35 3.507a.552.552 0 0 1-1.1 0z");
        builder.CloseElement(); // Close path
        builder.CloseElement(); // Close svg
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
        builder.CloseElement(); // Close path
        builder.OpenElement(10, "path");
        builder.AddAttribute(11, "d", "M7.002 12a1 1 0 1 1 2 0 1 1 0 0 1-2 0M7.1 5.995a.905.905 0 1 1 1.8 0l-.35 3.507a.552.552 0 0 1-1.1 0z");
        builder.CloseElement(); // Close path
        builder.CloseElement(); // Close svg
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
        builder.CloseElement(); // Close path
        builder.OpenElement(10, "path");
        builder.AddAttribute(11, "d", "m8.93 6.588-2.29.287-.082.38.45.083c.294.07.352.176.288.469l-.738 3.468c-.194.897.105 1.319.808 1.319.545 0 1.178-.252 1.465-.598l.088-.416c-.2.176-.492.246-.686.246-.275 0-.375-.193-.304-.533zM9 4.5a1 1 0 1 1-2 0 1 1 0 0 1 2 0");
        builder.CloseElement(); // Close path
        builder.CloseElement(); // Close svg
    };

    #endregion
}
