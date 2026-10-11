namespace Osirion.Blazor.Cms.Admin.Common.Constants;

/// <summary>
/// User-facing error texts. Exception details can contain tokens, paths or provider responses, so they are
/// written to the log and never shown in the admin UI.
/// </summary>
public static class UserMessages
{
    /// <summary>Shown when an operation fails for a reason the user cannot act on.</summary>
    public const string OperationFailed = "The operation failed. Please try again. If the problem continues, check the server log.";

    /// <summary>Shown when content could not be loaded.</summary>
    public const string LoadFailed = "The content could not be loaded. Please try again.";

    /// <summary>Shown when sign-in could not be completed.</summary>
    public const string SignInFailed = "Sign-in failed. Please try again.";

    /// <summary>Shown when branches could not be loaded or changed.</summary>
    public const string BranchOperationFailed = "The branch operation failed. Please try again.";

    /// <summary>Shown when a save could not be completed.</summary>
    public const string SaveFailed = "The content could not be saved. Please try again.";

    /// <summary>Shown when a search could not be completed.</summary>
    public const string SearchFailed = "The search could not be completed. Please try again.";

    /// <summary>Shown when a file could not be opened.</summary>
    public const string OpenFailed = "The file could not be opened. Please try again.";

    /// <summary>Shown when content could not be deleted.</summary>
    public const string DeleteFailed = "The content could not be deleted. Please try again.";
}
