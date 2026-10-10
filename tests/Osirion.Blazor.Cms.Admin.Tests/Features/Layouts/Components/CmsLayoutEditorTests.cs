using Bunit;
using Osirion.Blazor.Cms.Admin.Features.Layouts.Components;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.Layouts.Components;

// Before the fix CmsLayoutEditor declared a second "Theme" parameter next to OsirionComponentBase.Theme, so Blazor
// threw on its first render.
public class CmsLayoutEditorTests : AdminComponentTestContext
{
    [Fact]
    public void CmsLayoutEditor_Render_ShowsTitleWithLightTheme()
    {
        RenderInteractively();

        var cut = Render<CmsLayoutEditor>(parameters => parameters
            .Add(p => p.Title, "Edit post"));

        cut.Find(".osirion-editor-breadcrumb-current").TextContent.ShouldBe("Edit post");
        cut.Find(".osirion-editor-layout").ClassList.ShouldContain("osirion-admin-theme-light");
    }
}
