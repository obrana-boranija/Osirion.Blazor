using Osirion.Blazor.Cms.Domain.Common;
using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Domain.Tests.Common;

public class EntityEqualityTests
{
    private sealed class Probe : Entity<string>
    {
        public Probe(string? id) : base(id!) { }
    }

    private sealed class OtherProbe : Entity<string>
    {
        public OtherProbe(string id) : base(id) { }
    }

    [Fact]
    public void Equals_SameIdDifferentRuntimeType_ReturnsFalse()
    {
        var content = ContentItem.Create("same", "Title", "Body", "a.md", "provider");
        var directory = DirectoryItem.Create("same", "dir", "Dir", "provider");

        Assert.False(content.Equals((object)directory));
        Assert.False(new Probe("x").Equals(new OtherProbe("x")));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Equals_TwoTransientEntities_AreNotEqual(string? id)
    {
        var first = new Probe(id);
        var second = new Probe(id);

        Assert.False(first.Equals(second));
        Assert.True(first != second);
        Assert.Equal(2, new List<Probe> { first }.Concat(new[] { second }).Distinct().Count());
    }

    [Fact]
    public void Equals_TransientEntityWithItself_ReturnsTrue()
    {
        var entity = new Probe(null);

        Assert.True(entity.Equals(entity));
        Assert.Equal(entity.GetHashCode(), entity.GetHashCode());
    }

    [Fact]
    public void Equals_SameIdSameType_StillEqualWithMatchingHashCodes()
    {
        var first = new Probe("id");
        var second = new Probe("id");

        Assert.True(first.Equals(second));
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }
}
