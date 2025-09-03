# CmsAdminDashboard

Purpose
Main dashboard for the CMS admin showing statistics, recent activity and quick actions.

Parameters
- `Stats`: DashboardStats - Aggregated statistics shown on the dashboard
- `RecentActivity`: IEnumerable<ActivityItem> - Recent activity items
- `QuickActions`: IEnumerable<ActionItem> - Quick action items

Example

```razor
<CmsAdminDashboard Stats="@stats" RecentActivity="@activity" QuickActions="@actions" />

@code {
    private DashboardStats stats = new DashboardStats();
    private IEnumerable<ActivityItem> activity = Enumerable.Empty<ActivityItem>();
    private IEnumerable<ActionItem> actions = Enumerable.Empty<ActionItem>();
}
```
