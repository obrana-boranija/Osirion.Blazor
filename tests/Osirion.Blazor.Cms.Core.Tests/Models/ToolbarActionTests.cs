using Shouldly;

namespace Osirion.Blazor.Cms.Core.Tests.Models;

public class ToolbarActionTests
{
    [Fact]
    public void ToolbarAction_ShouldInitialize_WithCorrectValues()
    {
        // Arrange
        const string label = "Test";
        const ToolbarActionType actionType = ToolbarActionType.Wrap;
        const string action = "**|**|bold";
        const string title = "Format Bold";
        const string icon = "<svg>...</svg>";

        // Act
        var toolbarAction = new ToolbarAction(label, actionType, action, title, icon);

        // Assert
        toolbarAction.Label.ShouldBe(label);
        toolbarAction.ActionType.ShouldBe(actionType);
        toolbarAction.Action.ShouldBe(action);
        toolbarAction.Title.ShouldBe(title);
        toolbarAction.Icon.ShouldBe(icon);
    }

    [Fact]
    public void ToolbarAction_ShouldInitialize_WithNullIcon()
    {
        // Arrange & Act
        var toolbarAction = new ToolbarAction("Test", ToolbarActionType.Insert, "text", "Insert Text");

        // Assert
        toolbarAction.Icon.ShouldBeNull();
    }

    [Fact]
    public void ToolbarAction_ShouldHaveCorrectTypes()
    {
        // Assert - verify enum values
        Enum.GetValues(typeof(ToolbarActionType)).Length.ShouldBe(3);

        ((int)ToolbarActionType.Insert).ShouldBe(0);
        ((int)ToolbarActionType.Wrap).ShouldBe(1);
        ((int)ToolbarActionType.Custom).ShouldBe(2);
    }

    [Theory]
    [InlineData("Bold", ToolbarActionType.Wrap, "**|**|bold text", "Make text bold")]
    [InlineData("Italic", ToolbarActionType.Wrap, "*|*|italic text", "Make text italic")]
    [InlineData("Link", ToolbarActionType.Wrap, "[|](url)|link text", "Insert link")]
    [InlineData("Image", ToolbarActionType.Insert, "![alt text](url)", "Insert image")]
    [InlineData("List", ToolbarActionType.Insert, "- Item 1\n- Item 2\n- Item 3", "Insert bullet list")]
    public void ToolbarAction_ShouldWork_WithVariousOptions(string label, ToolbarActionType type, string action, string title)
    {
        // Arrange & Act
        var toolbarAction = new ToolbarAction(label, type, action, title);

        // Assert
        toolbarAction.Label.ShouldBe(label);
        toolbarAction.ActionType.ShouldBe(type);
        toolbarAction.Action.ShouldBe(action);
        toolbarAction.Title.ShouldBe(title);
    }
}
