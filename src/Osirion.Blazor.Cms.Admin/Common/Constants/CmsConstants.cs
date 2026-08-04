namespace Osirion.Blazor.Cms.Admin.Common.Constants;

/// <summary>Defines shared CMS administrator constants.</summary>
public static class CmsConstants
{
    // State storage keys
    /// <summary>Defines browser storage keys.</summary>
    public static class StorageKeys
    {
        /// <summary>The selected repository key.</summary>
        public const string SelectedRepository = "osirion_cms_selected_repository";
        /// <summary>The selected branch key.</summary>
        public const string SelectedBranch = "osirion_cms_selected_branch";
        /// <summary>The current path key.</summary>
        public const string CurrentPath = "osirion_cms_current_path";
        /// <summary>The authentication token key.</summary>
        public const string AuthToken = "osirion_cms_auth_token";
        /// <summary>The theme key.</summary>
        public const string Theme = "osirion_cms_theme";
    }

    // File types
    /// <summary>Defines supported content file extensions.</summary>
    public static class FileTypes
    {
        /// <summary>The Markdown extension.</summary>
        public const string Markdown = ".md";
        /// <summary>The JSON extension.</summary>
        public const string Json = ".json";
        /// <summary>The YAML extension.</summary>
        public const string Yaml = ".yaml";
        /// <summary>The alternate YAML extension.</summary>
        public const string Yml = ".yml";
    }

    // Event names
    /// <summary>Defines CMS event names.</summary>
    public static class Events
    {
        /// <summary>The repository-selected event.</summary>
        public const string RepositorySelected = "repository_selected";
        /// <summary>The branch-selected event.</summary>
        public const string BranchSelected = "branch_selected";
        /// <summary>The content-selected event.</summary>
        public const string ContentSelected = "content_selected";
        /// <summary>The content-saved event.</summary>
        public const string ContentSaved = "content_saved";
        /// <summary>The content-deleted event.</summary>
        public const string ContentDeleted = "content_deleted";
        /// <summary>The authentication-changed event.</summary>
        public const string AuthenticationChanged = "authentication_changed";
    }

    // Route paths
    /// <summary>Defines CMS administration routes.</summary>
    public static class Routes
    {
        /// <summary>The login route.</summary>
        public const string Login = "/osirion/login";
        /// <summary>The dashboard route.</summary>
        public const string Dashboard = "/osirion";
        /// <summary>The content browser route.</summary>
        public const string ContentBrowser = "/osirion/content";
        /// <summary>The content editor route.</summary>
        public const string ContentEditor = "/osirion/content/edit";
        /// <summary>The settings route.</summary>
        public const string Settings = "/osirion/settings";
    }
}
