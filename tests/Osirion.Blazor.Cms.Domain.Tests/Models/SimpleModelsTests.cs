using Osirion.Blazor.Cms.Domain.Models;

namespace Osirion.Blazor.Cms.Domain.Tests.Models;

public class SimpleModelsTests
{
    [Fact]
    public void ShareLink_Properties_ShouldBeSettable()
    {
        // Arrange & Act
        var shareLink = new ShareLink
        {
            Name = "Twitter",
            Label = "Share on Twitter",
            Url = "https://twitter.com/intent/tweet?url={0}&text={1}",
            Icon = "<svg>...</svg>"
        };

        // Assert
        Assert.Equal("Twitter", shareLink.Name);
        Assert.Equal("Share on Twitter", shareLink.Label);
        Assert.Equal("https://twitter.com/intent/tweet?url={0}&text={1}", shareLink.Url);
        Assert.Equal("<svg>...</svg>", shareLink.Icon);
    }

    [Fact]
    public void ShareLink_DefaultValues_ShouldBeEmptyStrings()
    {
        // Arrange & Act
        var shareLink = new ShareLink();

        // Assert
        Assert.Equal(string.Empty, shareLink.Name);
        Assert.Equal(string.Empty, shareLink.Label);
        Assert.Equal(string.Empty, shareLink.Url);
        Assert.Equal(string.Empty, shareLink.Icon);
    }

    [Fact]
    public void TextSelection_Properties_ShouldBeSettable()
    {
        // Arrange & Act
        var selection = new TextSelection
        {
            Text = "Selected text",
            Start = 10,
            End = 22
        };

        // Assert
        Assert.Equal("Selected text", selection.Text);
        Assert.Equal(10, selection.Start);
        Assert.Equal(22, selection.End);
    }

    [Fact]
    public void TextSelection_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var selection = new TextSelection();

        // Assert
        Assert.Equal(string.Empty, selection.Text);
        Assert.Equal(0, selection.Start);
        Assert.Equal(0, selection.End);
    }
}