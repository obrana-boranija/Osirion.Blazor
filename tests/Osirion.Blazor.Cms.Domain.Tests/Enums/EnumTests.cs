using Osirion.Blazor.Cms.Domain.Enums;

namespace Osirion.Blazor.Cms.Domain.Tests.Enums;

public class EnumTests
{
    [Fact]
    public void ContentStatus_HasExpectedValues()
    {
        // Assert
        Assert.Equal(5, Enum.GetValues(typeof(ContentStatus)).Length);
        Assert.Equal(0, (int)ContentStatus.Draft);
        Assert.Equal(1, (int)ContentStatus.Published);
        Assert.Equal(2, (int)ContentStatus.Scheduled);
        Assert.Equal(3, (int)ContentStatus.Archived);
        Assert.Equal(4, (int)ContentStatus.InReview);
    }

    [Fact]
    public void SortField_HasExpectedValues()
    {
        // Assert
        Assert.Equal(9, Enum.GetValues(typeof(SortField)).Length);
        Assert.Equal(0, (int)SortField.Date);
        Assert.Equal(1, (int)SortField.Title);
        Assert.Equal(2, (int)SortField.Author);
        Assert.Equal(3, (int)SortField.LastModified);
        Assert.Equal(4, (int)SortField.Order);
        Assert.Equal(5, (int)SortField.Created);
        Assert.Equal(6, (int)SortField.PublishDate);
        Assert.Equal(7, (int)SortField.Slug);
        Assert.Equal(8, (int)SortField.ReadTime);
    }

    [Fact]
    public void SortDirection_HasExpectedValues()
    {
        // Assert
        Assert.Equal(2, Enum.GetValues(typeof(SortDirection)).Length);
        Assert.Equal(0, (int)SortDirection.Ascending);
        Assert.Equal(1, (int)SortDirection.Descending);
    }
}