using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Services;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.ViewModels;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.ContentEditor;

public class ContentEditorModuleTests
{
    [Fact]
    public void AddContentEditor_RegistersServiceAndViewModelAsScoped()
    {
        var services = new ServiceCollection();

        var returned = services.AddContentEditor();

        returned.ShouldBeSameAs(services);
        services.ShouldContain(d => d.ServiceType == typeof(ContentEditorService) && d.Lifetime == ServiceLifetime.Scoped);
        services.ShouldContain(d => d.ServiceType == typeof(ContentEditorViewModel) && d.Lifetime == ServiceLifetime.Scoped);
    }
}
