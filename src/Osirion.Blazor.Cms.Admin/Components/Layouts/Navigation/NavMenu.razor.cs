using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Cms.Admin.Components.Layouts.Navigation;
/// <summary>Provides navigation links and account actions for the admin interface.</summary>
public partial class NavMenu
{
    /// <summary>Gets or sets the authenticated user displayed by the menu.</summary>
    [Parameter]
    public UserInfo? User { get; set; }

    /// <summary>Gets or sets the callback invoked when the user signs out.</summary>
    [Parameter]
    public EventCallback OnSignOut { get; set; }

    /// <summary>Contains the account details needed to render the navigation menu.</summary>
    public class UserInfo
    {
        /// <summary>Gets or sets the user's display name.</summary>
        public string? Username { get; set; }
        /// <summary>Gets or sets the user's email address.</summary>
        public string? Email { get; set; }
        /// <summary>Gets or sets a value indicating whether the user is authenticated.</summary>
        public bool IsAuthenticated { get; set; }
    }
}
