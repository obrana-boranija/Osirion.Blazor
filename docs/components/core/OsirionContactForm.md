# OsirionContactForm

Purpose
Accessible contact form with optional contact info panel, privacy and subscription options, SSR-friendly form post pattern.

Key parameters
- Id (required), FormTitle, FormDescription
- NamePlaceholder, EmailPlaceholder, SubjectPlaceholder, MessagePlaceholder, MessageRows
- SubmitButtonText, SubmittingText, ShowResetButton, ResetButtonText
- PrivacyPolicyText (null to hide)
- ShowSubscribeOption, SubscribeText
- ShowContactInformation, ContactInfoTitle, ContactInfo
- SideBySideLayout (default true), UseDarkTheme
- UseBuiltInEmailService (requires EmailServiceFactory registration)

Events
- OnSubmit(ContactFormModel)
- OnReset()
- OnValidationFailed(EditContext)

SSR behavior
- Uses SupplyParameterFromForm and query parameter SubmissionResultValue to display status after navigation

Notes
- Use GetAlertClass styles from OsirionComponentBase
- Add EmailOptions and a service factory to enable server email delivery.
