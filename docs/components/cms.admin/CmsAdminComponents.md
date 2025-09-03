# CMS Admin Components

Overview
Administrative UI for content repositories (e.g., GitHub). Key features:
- Authentication guard (AuthGuard)
- Repository selection and branch management
- Content browser and editor with metadata forms
- Commit panel and preview
- Admin dashboard and settings

Primary components
- ContentEditor: orchestrates editing flow
  - MarkdownEditorWithPreview
  - BasicMetadataForm, AdvancedMetadataForm, SeoMetadataForm, SocialMetadataForm, TagInput
  - CommitPanel, MetadataPreview
- ContentBrowser/FileExplorer: navigate repository contents
- RepositorySelector, BranchSelector
- CmsAdminDashboard
- AuthGuard, Login
- Admin layouts: AdminLayout, AdminPage, CmsLayoutEditor, NavMenu, Breadcrumb

Component docs
- `MarkdownEditorWithPreview.md` - Markdown editor with live preview and editing helpers
- `CommitPanel.md` - Commit and save UI for repository commits
- `MetadataEditor.md` - Composite metadata editor orchestration
- `MetadataPreview.md` - Read-only preview of metadata
- `SeoMetadataForm.md` - SEO metadata editor
- `SocialMetadataForm.md` - Social media metadata editor (OpenGraph/Twitter)
- `TagInput.md` - Reusable tag input component
- `BasicMetadataForm.md` - Compact metadata form for simple pages
- `AdvancedMetadataForm.md` - Full metadata editor including structured data

Pages and layouts
- `DashboardPage.md` - Admin landing/dashboard page
- `EditContentPage.md` - Page hosting ContentEditor for editing content
- `LoginPage.md` - Authentication/login page for admin
- `SettingsPage.md` - Administrative settings page
- `OsirionAdminLayout.md` - Root admin layout documentation
- `Header.md` - Top header component
- `Footer.md` - Footer component
- `NavMenu.md` - Side navigation component
- `Card.md` - UI card wrapper
- `StatusAlert.md` - Inline status/notification component

Notes
- Built on feature modules with DI registration helpers
- Adapters for GitHub and other content backends
- See src/Osirion.Blazor.Cms.Admin for service extensions and options.
