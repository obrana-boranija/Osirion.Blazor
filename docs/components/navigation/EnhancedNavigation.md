# EnhancedNavigation

Purpose
Client-side enhanced navigation hook for Blazor enhancedload, controlling scroll reset behavior.

Key parameters
- Behavior: Auto, Smooth, Instant
- ResetScrollOnNavigation (default true)
- PreserveScrollForSamePageNavigation (default true)

Notes
- Emits a small script content that hooks Blazor.addEventListener('enhancedload')
- For forms that navigate with query result (e.g., SubscriptionCard), consider ResetScrollOnNavigation=false.
