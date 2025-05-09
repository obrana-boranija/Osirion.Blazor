using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Cms.Admin.Components.Layouts.Navigation;
public partial class NavMenu
{
    [Parameter]
    public UserInfo? User { get; set; }

    [Parameter]
    public EventCallback OnSignOut { get; set; }

    public class UserInfo
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}
