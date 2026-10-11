using Bunit;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components.Shared;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.ContentEditor.Components.Shared;

public class MetadataEditorTests : AdminComponentTestContext
{
    private readonly List<FrontMatter> _metadataChanges = [];
    private readonly List<SeoMetadata> _seoChanges = [];

    public MetadataEditorTests()
    {
        RenderInteractively();
    }

    private static FrontMatter CreateMetadata()
    {
        var metadata = FrontMatter.Create("Post Title", "Post description", new DateTime(2024, 1, 15));
        metadata.FeaturedImage = "https://site.test/f.png";
        return metadata;
    }

    private IRenderedComponent<MetadataEditor> RenderEditor(FrontMatter metadata, SeoMetadata seo) =>
        Render<MetadataEditor>(parameters => parameters
            .Add(p => p.Metadata, metadata)
            .Add(p => p.SeoData, seo)
            .Add(p => p.MetadataChanged, (FrontMatter value) => _metadataChanges.Add(value))
            .Add(p => p.SeoMetadataChanged, (SeoMetadata value) => _seoChanges.Add(value)));

    private static void OpenTab(IRenderedComponent<MetadataEditor> cut, int index) =>
        cut.FindAll("button.nav-link")[index].Click();

    [Fact]
    public void MetadataEditor_StaticRender_ShowsBasicTabAndPreview()
    {
        SetRendererInfo(new RendererInfo("Static", false));

        var cut = Render<MetadataEditor>(parameters => parameters.Add(p => p.Metadata, CreateMetadata()));

        cut.FindAll("button.nav-link")[0].ClassList.ShouldContain("active");
        cut.FindComponent<BasicMetadataForm>().ShouldNotBeNull();
        cut.FindComponents<MetadataPreview>().ShouldHaveSingleItem();
        cut.FindAll(".mobile-preview-overlay").ShouldBeEmpty();
    }

    [Fact]
    public void MetadataEditor_EmptySeo_IsFilledFromFrontMatter()
    {
        var cut = RenderEditor(CreateMetadata(), SeoMetadata.Create("", ""));

        var seo = cut.Instance.SeoData;
        seo.Title.ShouldBe("Post Title");
        seo.Description.ShouldBe("Post description");
        seo.OgTitle.ShouldBe("Post Title");
        seo.OgImageUrl.ShouldBe("https://site.test/f.png");
        seo.TwitterTitle.ShouldBe("Post Title");
    }

    [Fact]
    public void MetadataEditor_EmptySeoWithoutImage_UsesEmptyImage()
    {
        var metadata = FrontMatter.Create("Post Title");

        var cut = RenderEditor(metadata, SeoMetadata.Create("", ""));

        cut.Instance.SeoData.OgImageUrl.ShouldBe(string.Empty);
        cut.Instance.SeoData.TwitterImageUrl.ShouldBe(string.Empty);
    }

    [Fact]
    public void MetadataEditor_ExistingSeo_IsKept()
    {
        var cut = RenderEditor(CreateMetadata(), SeoMetadata.Create("Own title", "Own description"));

        cut.Instance.SeoData.Title.ShouldBe("Own title");
    }

    [Fact]
    public void MetadataEditor_NullParameters_FallBackToDefaults()
    {
        var cut = RenderEditor(null!, null!);

        cut.Instance.Metadata.Title.ShouldBe("New Post");
        cut.Instance.SeoData.Title.ShouldBe("New Post");
    }

    [Fact]
    public void MetadataEditor_EachTab_ShowsItsForm()
    {
        var cut = RenderEditor(CreateMetadata(), SeoMetadata.Create("", ""));

        OpenTab(cut, 1);
        cut.FindComponent<SeoMetadataForm>().ShouldNotBeNull();
        cut.FindAll("button.nav-link")[1].ClassList.ShouldContain("active");

        OpenTab(cut, 2);
        cut.FindComponent<SocialMetadataForm>().ShouldNotBeNull();

        OpenTab(cut, 3);
        cut.FindComponent<AdvancedMetadataForm>().ShouldNotBeNull();

        OpenTab(cut, 0);
        cut.FindComponent<BasicMetadataForm>().ShouldNotBeNull();
    }

    [Fact]
    public void MetadataEditor_TitleTypedWhileSeoTitleIsEmpty_SyncsSeoAndRaisesChange()
    {
        var metadata = FrontMatter.Create(string.Empty, "Post description", new DateTime(2024, 1, 15));
        var cut = RenderEditor(metadata, SeoMetadata.Create("", ""));

        cut.Find("#title").Input("Typed");

        _metadataChanges[^1].Title.ShouldBe("Typed");
        cut.Instance.SeoData.Title.ShouldBe("Typed");
        cut.Instance.SeoData.Description.ShouldBe("Post description");
    }

    // Pins current behavior: the form edits the front matter in place, so the "titles match" check compares the
    // new title with itself only after the SEO title has already diverged, and an auto-filled SEO title stops
    // following the post title after the first keystroke.
    [Fact]
    public void MetadataEditor_TitleEditedAfterSeoWasAutoFilled_KeepsTheEarlierSeoTitle()
    {
        var cut = RenderEditor(CreateMetadata(), SeoMetadata.Create("", ""));

        cut.Find("#title").Input("Renamed");

        _metadataChanges[^1].Title.ShouldBe("Renamed");
        cut.Instance.SeoData.Title.ShouldBe("Post Title");
    }

    [Fact]
    public void MetadataEditor_TitleEditedWithCustomSeoTitle_KeepsSeoTitle()
    {
        var cut = RenderEditor(CreateMetadata(), SeoMetadata.Create("Custom SEO", "Custom"));

        cut.Find("#title").Input("Renamed");

        _metadataChanges[^1].Title.ShouldBe("Renamed");
        cut.Instance.SeoData.Title.ShouldBe("Custom SEO");
    }

    [Fact]
    public void MetadataEditor_MetadataEditedWithoutListener_StillUpdatesSeo()
    {
        var cut = Render<MetadataEditor>(parameters => parameters
            .Add(p => p.Metadata, CreateMetadata())
            .Add(p => p.SeoData, SeoMetadata.Create("", "")));

        cut.Find("#description").Input("Changed");

        cut.Instance.SeoData.Description.ShouldBe("Changed");
    }

    [Fact]
    public void MetadataEditor_SeoEdited_RaisesSeoChange()
    {
        var cut = RenderEditor(CreateMetadata(), SeoMetadata.Create("", ""));
        OpenTab(cut, 1);

        cut.Find("#metaTitle").Input("Search title");

        _seoChanges[^1].Title.ShouldBe("Search title");
        cut.Instance.SeoData.Title.ShouldBe("Search title");
    }

    [Fact]
    public void MetadataEditor_SeoEditedWithoutListener_UpdatesSeo()
    {
        var cut = Render<MetadataEditor>(parameters => parameters.Add(p => p.Metadata, CreateMetadata()));
        OpenTab(cut, 2);

        cut.Find("#ogTitle").Input("Social title");

        cut.Instance.SeoData.OgTitle.ShouldBe("Social title");
    }

    [Fact]
    public void MetadataEditor_TogglePreview_OpensAndClosesMobilePreview()
    {
        var cut = RenderEditor(CreateMetadata(), SeoMetadata.Create("", ""));

        cut.Find("button[title='Toggle Preview']").Click();
        cut.FindAll(".mobile-preview-overlay").ShouldHaveSingleItem();
        cut.FindComponents<MetadataPreview>().Count.ShouldBe(2);

        cut.Find("button.btn-close").Click();
        cut.FindAll(".mobile-preview-overlay").ShouldBeEmpty();

        cut.Find("button[title='Toggle Preview']").Click();
        cut.Find(".mobile-preview-overlay").Click();
        cut.FindAll(".mobile-preview-overlay").ShouldBeEmpty();
    }

    [Fact]
    public void MetadataEditor_PreviewRefresh_RaisesRefresh()
    {
        var refreshed = 0;
        var cut = Render<MetadataEditor>(parameters => parameters
            .Add(p => p.Metadata, CreateMetadata())
            .Add(p => p.OnRefresh, () => refreshed++));

        cut.Find("button[title='Refresh preview']").Click();

        refreshed.ShouldBe(1);
    }

    [Fact]
    public void MetadataEditor_PreviewRefreshWithoutListener_KeepsEditor()
    {
        var cut = Render<MetadataEditor>(parameters => parameters.Add(p => p.Metadata, CreateMetadata()));

        cut.Find("button[title='Refresh preview']").Click();

        cut.Find(".metadata-editor").ShouldNotBeNull();
    }

    [Fact]
    public void MetadataEditor_WithoutActions_HidesPreviewRefresh()
    {
        var cut = Render<MetadataEditor>(parameters => parameters
            .Add(p => p.Metadata, CreateMetadata())
            .Add(p => p.ShowActions, false));

        cut.FindAll("button[title='Refresh preview']").ShouldBeEmpty();
    }
}
