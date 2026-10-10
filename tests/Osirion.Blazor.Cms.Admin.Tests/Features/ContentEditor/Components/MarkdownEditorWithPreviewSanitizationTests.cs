using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components.Shared;
using Osirion.Blazor.Cms.Infrastructure.DependencyInjection;
using Osirion.Blazor.Components;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.ContentEditor.Components;

public class MarkdownEditorWithPreviewSanitizationTests : BunitContext
{
    public MarkdownEditorWithPreviewSanitizationTests()
    {
        Services.AddSingleton(new CmsState());
        Services.AddSingleton(Substitute.For<IEventPublisher>());
        Services.AddSingleton(Substitute.For<IEventSubscriber>());
        Services.AddSingleton(Microsoft.Extensions.Options.Options.Create(new ThemingOptions()));
        Services.AddMarkdownServices();
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    // The admin preview runs in an interactive session, so a handler in repository content would run with the
    // editor's privileges. Fails before the fix: neither the markdown processor nor the HTML renderer
    // sanitized by default.
    [Fact]
    public void MarkdownEditorWithPreview_ContentWithScriptAndHandler_RendersPreviewWithoutThem()
    {
        SetRendererInfo(new RendererInfo("Server", true));

        var cut = Render<MarkdownEditorWithPreview>(parameters => parameters
            .Add(p => p.Content, "Text <img src=\"/a.png\" onerror=\"alert(1)\">\n\n<script>alert(2)</script>"));

        var preview = cut.Find(".preview-container article");
        preview.QuerySelectorAll("script").ShouldBeEmpty();
        preview.QuerySelector("img").ShouldNotBeNull().HasAttribute("onerror").ShouldBeFalse();
    }
}
