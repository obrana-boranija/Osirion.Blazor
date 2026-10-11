using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components.Shared;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.ContentEditor.Components.Shared;

public class TagInputTests : AdminComponentTestContext
{
    private readonly List<List<string>> _changes = [];

    public TagInputTests()
    {
        RenderInteractively();
    }

    private IRenderedComponent<TagInput> RenderTagInput(List<string> tags, int maxTags = 0) =>
        Render<TagInput>(parameters => parameters
            .Add(p => p.Tags, tags)
            .Add(p => p.MaxTags, maxTags)
            .Add(p => p.TagsChanged, (List<string> changed) => _changes.Add([.. changed])));

    private static void Type(IRenderedComponent<TagInput> cut, string text, string key)
    {
        cut.Find("input.tag-input").Input(text);
        cut.Find("input.tag-input").KeyDown(new KeyboardEventArgs { Key = key });
    }

    [Fact]
    public void TagInput_StaticRender_ShowsTagsAndPlaceholderOnlyWhenEmpty()
    {
        SetRendererInfo(new RendererInfo("Static", false));

                var empty = Render<TagInput>(parameters => parameters.Add(p => p.Placeholder, "Add one"));
        var filled = Render<TagInput>(parameters => parameters.Add(p => p.Tags, ["a", "b"]));

        empty.Find("input.tag-input").GetAttribute("placeholder").ShouldBe("Add one");
        empty.FindAll(".tag-counter").ShouldBeEmpty();
        filled.FindAll(".tag-chip").Count.ShouldBe(2);
        filled.Find("input.tag-input").GetAttribute("placeholder").ShouldBe(string.Empty);
    }

    [Fact]
    public void TagInput_WithMaxTags_ShowsCounter()
    {
        var cut = RenderTagInput(["a"], maxTags: 5);

        cut.Find(".tag-counter").TextContent.ShouldContain("1 / 5 tags");
    }

    [Theory]
    [InlineData("Enter")]
    [InlineData(",")]
    public void TagInput_EnterOrComma_AddsTrimmedTagAndClearsInput(string key)
    {
        var cut = RenderTagInput([]);

        Type(cut, "  blazor  ", key);

        _changes.ShouldHaveSingleItem().ShouldBe(["blazor"]);
        cut.FindAll(".tag-chip").ShouldHaveSingleItem().TextContent.ShouldContain("blazor");
        cut.Find("input.tag-input").GetAttribute("value").ShouldBe(string.Empty);
    }

    [Fact]
    public void TagInput_BlankInput_DoesNotAddTag()
    {
        var cut = RenderTagInput([]);

        Type(cut, "   ", "Enter");

        _changes.ShouldBeEmpty();
        cut.FindAll(".tag-chip").ShouldBeEmpty();
    }

    [Fact]
    public void TagInput_AtMaxTags_DoesNotAddTag()
    {
        var cut = RenderTagInput(["a", "b"], maxTags: 2);

        Type(cut, "c", "Enter");

        _changes.ShouldBeEmpty();
        cut.FindAll(".tag-chip").Count.ShouldBe(2);
    }

    [Fact]
    public void TagInput_DuplicateIgnoringCase_ClearsInputWithoutAdding()
    {
        var cut = RenderTagInput(["Blazor"]);

        Type(cut, "blazor", "Enter");

        _changes.ShouldBeEmpty();
        cut.FindAll(".tag-chip").ShouldHaveSingleItem();
        cut.Find("input.tag-input").GetAttribute("value").ShouldBe(string.Empty);
    }

    [Fact]
    public void TagInput_NewTagWithoutListener_AddsTag()
    {
        var cut = Render<TagInput>(parameters => parameters.Add(p => p.Tags, []));

        Type(cut, "solo", "Enter");

        cut.FindAll(".tag-chip").ShouldHaveSingleItem().TextContent.ShouldContain("solo");
    }

    [Fact]
    public void TagInput_BackspaceOnEmptyInput_RemovesLastTag()
    {
        var cut = RenderTagInput(["a", "b"]);

        Type(cut, string.Empty, "Backspace");

        _changes.ShouldHaveSingleItem().ShouldBe(["a"]);
        cut.FindAll(".tag-chip").ShouldHaveSingleItem();
    }

    [Fact]
    public void TagInput_BackspaceWithText_KeepsTags()
    {
        var cut = RenderTagInput(["a"]);

        Type(cut, "x", "Backspace");

        _changes.ShouldBeEmpty();
        cut.FindAll(".tag-chip").ShouldHaveSingleItem();
    }

    [Fact]
    public void TagInput_BackspaceWithoutTags_DoesNothing()
    {
        var cut = RenderTagInput([]);

        Type(cut, string.Empty, "Backspace");

        _changes.ShouldBeEmpty();
    }

    [Fact]
    public void TagInput_OtherKey_DoesNotAddTag()
    {
        var cut = RenderTagInput([]);

        Type(cut, "abc", "a");

        _changes.ShouldBeEmpty();
        cut.Find("input.tag-input").GetAttribute("value").ShouldBe("abc");
    }

    [Fact]
    public void TagInput_RemoveButton_RemovesThatTag()
    {
        var cut = RenderTagInput(["a", "b"]);

        cut.FindAll("button.tag-remove")[0].Click();

        _changes.ShouldHaveSingleItem().ShouldBe(["b"]);
    }

    [Fact]
    public void TagInput_RemoveWithoutListener_RemovesTag()
    {
        var cut = Render<TagInput>(parameters => parameters.Add(p => p.Tags, ["a"]));

        cut.Find("button.tag-remove").Click();

        cut.FindAll(".tag-chip").ShouldBeEmpty();
    }

    [Fact]
    public void TagInput_ContainerClick_FocusesInput()
    {
        var cut = RenderTagInput([]);

        cut.Find(".tag-input-container").Click();

        JSInterop.VerifyFocusAsyncInvoke().Arguments.ShouldNotBeEmpty();
    }
}
