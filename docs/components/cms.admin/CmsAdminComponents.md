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

Notes
- Built on feature modules with DI registration helpers
- Adapters for GitHub and other content backends
- See src/Osirion.Blazor.Cms.Admin for service extensions and options.
