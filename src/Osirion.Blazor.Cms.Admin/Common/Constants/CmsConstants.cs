namespace Osirion.Blazor.Cms.Admin.Common.Constants;

public static class CmsConstants
{
    // State storage keys
    public static class StorageKeys
    {
        public const string SelectedRepository = "osirion_cms_selected_repository";
        public const string SelectedBranch = "osirion_cms_selected_branch";
        public const string CurrentPath = "osirion_cms_current_path";
        public const string AuthToken = "osirion_cms_auth_token";
        public const string Theme = "osirion_cms_theme";
    }

    // File types
    public static class FileTypes
    {
        public const string Markdown = ".md";
        public const string Json = ".json";
        public const string Yaml = ".yaml";
        public const string Yml = ".yml";
    }

    // Event names
    public static class Events
    {
        public const string RepositorySelected = "repository_selected";
        public const string BranchSelected = "branch_selected";
        public const string ContentSelected = "content_selected";
        public const string ContentSaved = "content_saved";
        public const string ContentDeleted = "content_deleted";
        public const string AuthenticationChanged = "authentication_changed";
    }

    // Route paths
    public static class Routes
    {
        public const string Login = "/osirion/login";
        public const string Dashboard = "/osirion";
        public const string ContentBrowser = "/osirion/content";
        public const string ContentEditor = "/osirion/content/edit";
        public const string Settings = "/osirion/settings";
    }
}