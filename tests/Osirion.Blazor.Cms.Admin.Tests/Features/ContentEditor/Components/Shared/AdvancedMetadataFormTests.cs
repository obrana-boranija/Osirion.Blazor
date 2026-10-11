using System.Text;
using System.Text.Json;
using Bunit;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components.Shared;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.ContentEditor.Components.Shared;

public class AdvancedMetadataFormTests : AdminComponentTestContext
{
    private readonly List<FrontMatter> _changes = [];

    public AdvancedMetadataFormTests()
    {
        RenderInteractively();
    }

    private IRenderedComponent<AdvancedMetadataForm> RenderForm(FrontMatter metadata) =>
        Render<AdvancedMetadataForm>(parameters => parameters
            .Add(p => p.Metadata, metadata)
            .Add(p => p.SeoMetadata, SeoMetadata.Create("Seo title", "Seo description"))
            .Add(p => p.MetadataChanged, (FrontMatter value) => _changes.Add(value)));

    private static string DecodeDownload(JSRuntimeInvocation invocation) =>
        Encoding.UTF8.GetString(Convert.FromBase64String((string)invocation.Arguments[1]!));

    [Fact]
    public void AdvancedMetadataForm_StaticRender_ShowsExistingCustomFields()
    {
        SetRendererInfo(new RendererInfo("Static", false));
        var metadata = FrontMatter.Create("Post");
        metadata.CustomFields = new Dictionary<string, object> { ["color"] = "blue" };

        var cut = Render<AdvancedMetadataForm>(parameters => parameters.Add(p => p.Metadata, metadata));

        var inputs = cut.FindAll(".custom-field-item input");
        inputs[0].GetAttribute("value").ShouldBe("color");
        inputs[1].GetAttribute("value").ShouldBe("blue");
    }

    [Fact]
    public void AdvancedMetadataForm_NullParameters_FallBackToDefaults()
    {
        var cut = Render<AdvancedMetadataForm>(parameters => parameters
            .Add(p => p.Metadata, null!)
            .Add(p => p.SeoMetadata, null!));

        cut.Instance.Metadata.Title.ShouldBe("New Post");
        cut.Instance.SeoMetadata.ShouldNotBeNull();
        cut.FindAll(".custom-field-item").ShouldBeEmpty();
    }

    [Fact]
    public void AdvancedMetadataForm_GenerateSlug_BuildsSlugFromTitle()
    {
        var cut = RenderForm(FrontMatter.Create("Tom's \"Best\" Q&A -- 2024!"));

        cut.Find("label[for='slug'] button").Click();

        _changes.ShouldHaveSingleItem().Slug.ShouldBe("toms-best-qanda-2024");
        cut.Find("#slug").GetAttribute("value").ShouldBe("toms-best-qanda-2024");
    }

    [Fact]
    public void AdvancedMetadataForm_GenerateSlugWithoutTitle_DoesNothing()
    {
        var cut = RenderForm(FrontMatter.Create(" "));

        cut.Find("label[for='slug'] button").Click();

        _changes.ShouldBeEmpty();
    }

    [Fact]
    public void AdvancedMetadataForm_GenerateSlugWithoutListener_UpdatesSlug()
    {
        var cut = Render<AdvancedMetadataForm>(parameters => parameters.Add(p => p.Metadata, FrontMatter.Create("A B")));

        cut.Find("label[for='slug'] button").Click();

        cut.Find("#slug").GetAttribute("value").ShouldBe("a-b");
    }

    [Fact]
    public void AdvancedMetadataForm_EditingFields_RaisesChangedWithNewValues()
    {
        var cut = RenderForm(FrontMatter.Create("Post"));

        cut.Find("#layout").Change("landing");
        cut.Find("#slug").Input("my-slug");
        cut.Find("#postId").Input("id-1");

        var last = _changes[^1];
        last.Layout.ShouldBe("landing");
        last.Slug.ShouldBe("my-slug");
        last.Id.ShouldBe("id-1");
    }

    [Fact]
    public void AdvancedMetadataForm_AddingCustomField_StoresNamedFieldsOnly()
    {
        var cut = RenderForm(FrontMatter.Create("Post"));

        cut.Find("button.btn-outline-primary").Click();
        cut.Find("button.btn-outline-primary").Click();
        var inputs = cut.FindAll(".custom-field-item input");
        inputs[0].Input("color");
        cut.FindAll(".custom-field-item input")[1].Input("red");

        cut.FindAll(".custom-field-item").Count.ShouldBe(2);
        _changes[^1].CustomFields.ShouldHaveSingleItem().Key.ShouldBe("color");
        _changes[^1].CustomFields["color"].ShouldBe("red");
    }

    [Fact]
    public void AdvancedMetadataForm_RemovingCustomField_DropsItFromMetadata()
    {
        var metadata = FrontMatter.Create("Post");
        metadata.CustomFields = new Dictionary<string, object> { ["color"] = "blue", ["size"] = 3 };
        var cut = RenderForm(metadata);

        cut.FindAll(".custom-field-item button.btn-outline-danger")[0].Click();

        cut.FindAll(".custom-field-item").ShouldHaveSingleItem();
        _changes.ShouldHaveSingleItem().CustomFields.Keys.ShouldBe(["size"]);
        _changes[0].CustomFields["size"].ShouldBe("3");
    }

    [Fact]
    public void AdvancedMetadataForm_ExportFrontMatter_DownloadsYaml()
    {
        var cut = RenderForm(FrontMatter.Create("Exported"));

        cut.FindAll(".d-flex.gap-2 button")[0].Click();

        var invocation = JSInterop.VerifyInvoke("downloadFile");
        invocation.Arguments[0].ShouldBe("frontmatter.yaml");
        invocation.Arguments[2].ShouldBe("text/yaml");
        DecodeDownload(invocation).ShouldContain("Exported");
    }

    [Fact]
    public void AdvancedMetadataForm_ExportJson_DownloadsFrontMatterAndSeo()
    {
        var cut = RenderForm(FrontMatter.Create("Exported"));

        cut.FindAll(".d-flex.gap-2 button")[1].Click();

        var invocation = JSInterop.VerifyInvoke("downloadFile");
        invocation.Arguments[0].ShouldBe("metadata.json");
        invocation.Arguments[2].ShouldBe("application/json");
        var json = JsonDocument.Parse(DecodeDownload(invocation)).RootElement;
        json.GetProperty("frontMatter").GetProperty("title").GetString().ShouldBe("Exported");
        json.GetProperty("seoMetadata").GetProperty("title").GetString().ShouldBe("Seo title");
    }
}
