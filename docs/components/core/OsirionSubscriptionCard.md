# OsirionSubscriptionCard

Purpose
Newsletter subscription card with optional categories and privacy text. SSR-friendly submission status pattern.

Key parameters
- Id (required), Title, Description
- ShowNameField, NamePlaceholder, EmailPlaceholder
- SubscribeButtonText, SubscribingText, ShowRequiredIndicators
- PrivacyPolicyText (null to hide)
- ShowSubscriptionCategories, SubscriptionCategories, CategoriesText
- ShowCategoriesAsDropdown, CategoriesDropdownText
- CompactLayout, UseDarkTheme, ShowBorder, ShowShadow
- UseBuiltInEmailService (optional)

Events
- OnSubscribe(SubscriptionModel)
- OnValidationFailed(EditContext)

SSR behavior
- Uses SupplyParameterFromForm and query SubmissionResultValue

Status messages
- SuccessMessage, ErrorMessage, SpamMessage, AlreadySubscribedMessage

Notes
- When categories enabled, required categories are always included
- For Static SSR with EnhancedNavigation, consider ResetScrollOnNavigation=false.
