using Bunit;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components.Shared;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.ContentEditor.Components.Shared;

public class CommitPanelTests : AdminComponentTestContext
{
    private const string CommitButton = "button.commit-panel-button-primary";
    private const string CancelButton = "button.commit-panel-button:not(.commit-panel-button-primary)";

    public CommitPanelTests()
    {
        RenderInteractively();
    }

    [Fact]
    public void CommitPanel_StaticRenderWithDefaults_ShowsTitleAndDisabledCommit()
    {
        SetRendererInfo(new RendererInfo("Static", false));

        var cut = Render<CommitPanel>();

        cut.Find(".commit-panel-title").TextContent.ShouldBe("Commit Changes");
        cut.Find(CommitButton).HasAttribute("disabled").ShouldBeTrue();
        cut.Find(CommitButton).TextContent.ShouldContain("Commit");
        cut.FindAll("textarea").ShouldBeEmpty();
        cut.FindAll(".commit-panel-error").ShouldBeEmpty();
    }

    [Fact]
    public void CommitPanel_WithCustomTextAndDescription_RendersThem()
    {
        var cut = Render<CommitPanel>(parameters => parameters
            .Add(p => p.Title, "Publish")
            .Add(p => p.CommitButtonText, "Ship it")
            .Add(p => p.ShowDescription, true)
            .Add(p => p.CommitMessage, "msg")
            .Add(p => p.CommitDescription, "details"));

        cut.Find(".commit-panel-title").TextContent.ShouldBe("Publish");
        cut.Find(CommitButton).TextContent.ShouldContain("Ship it");
        cut.Find(CommitButton).HasAttribute("disabled").ShouldBeFalse();
        cut.Find("#commitMessage").GetAttribute("value").ShouldBe("msg");
        cut.Find("textarea#commitDescription").GetAttribute("value").ShouldBe("details");
    }

    [Fact]
    public void CommitPanel_WhileCommitting_ShowsSpinnerAndDisablesButton()
    {
        var cut = Render<CommitPanel>(parameters => parameters
            .Add(p => p.CommitMessage, "msg")
            .Add(p => p.IsCommitting, true));

        cut.Find(CommitButton).HasAttribute("disabled").ShouldBeTrue();
        cut.Find(".commit-panel-loading-spinner").ShouldNotBeNull();
        cut.Find(CommitButton).TextContent.ShouldContain("Committing...");
    }

    [Fact]
    public void CommitPanel_CommitWithMessage_RaisesCommittingThenCommit()
    {
        var calls = new List<string>();
        var cut = Render<CommitPanel>(parameters => parameters
            .Add(p => p.CommitMessage, "msg")
            .Add(p => p.IsCommittingChanged, (bool value) => calls.Add($"committing:{value}"))
            .Add(p => p.OnCommitClicked, () => calls.Add("commit")));

        cut.Find(CommitButton).Click();

        calls.ShouldBe(["committing:True", "commit"]);
    }

    [Fact]
    public void CommitPanel_CommitWithoutCommitListener_StillRaisesCommitting()
    {
        bool? committing = null;
        var cut = Render<CommitPanel>(parameters => parameters
            .Add(p => p.CommitMessage, "msg")
            .Add(p => p.IsCommittingChanged, (bool value) => committing = value));

        cut.Find(CommitButton).Click();

        committing.ShouldBe(true);
    }

    [Fact]
    public void CommitPanel_CommitAfterMessageCleared_DoesNothing()
    {
        var calls = new List<string>();
        var cut = Render<CommitPanel>(parameters => parameters
            .Add(p => p.CommitMessage, "msg")
            .Add(p => p.IsCommittingChanged, (bool value) => calls.Add("committing"))
            .Add(p => p.OnCommitClicked, () => calls.Add("commit")));
        var button = cut.Find(CommitButton);
        cut.Find("#commitMessage").Input("   ");

        button.Click();

        calls.ShouldBeEmpty();
    }

    [Fact]
    public void CommitPanel_TypingMessageAndDescription_UpdatesFields()
    {
        var cut = Render<CommitPanel>(parameters => parameters.Add(p => p.ShowDescription, true));

        cut.Find("#commitMessage").Input("new message");
        cut.Find("#commitDescription").Input("new description");

        cut.Find(CommitButton).HasAttribute("disabled").ShouldBeFalse();
        cut.Find("#commitDescription").GetAttribute("value").ShouldBe("new description");
    }

    [Fact]
    public void CommitPanel_Cancel_RaisesCancel()
    {
        var cancelled = false;
        var cut = Render<CommitPanel>(parameters => parameters.Add(p => p.OnCancelClicked, () => cancelled = true));

        cut.Find(CancelButton).Click();

        cancelled.ShouldBeTrue();
    }

    [Fact]
    public void CommitPanel_CancelWithoutListener_KeepsPanel()
    {
        var cut = Render<CommitPanel>();

        cut.Find(CancelButton).Click();

        cut.Find(".commit-panel").ShouldNotBeNull();
    }

    [Fact]
    public async Task CommitPanel_FailureReported_ShowsErrorFooter()
    {
        var cut = Render<Probe>();

        await cut.InvokeAsync(() => cut.Instance.Fail());
        cut.Render();

        cut.Find(".commit-panel-error").TextContent.ShouldNotBeNullOrWhiteSpace();
    }

    private sealed class Probe : CommitPanel
    {
        public void Fail() => ReportFailure(new InvalidOperationException("boom"));
    }
}
