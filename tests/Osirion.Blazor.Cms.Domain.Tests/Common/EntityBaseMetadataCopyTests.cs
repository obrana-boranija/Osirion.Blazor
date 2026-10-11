using Osirion.Blazor.Cms.Domain.Common;

namespace Osirion.Blazor.Cms.Domain.Tests.Common;

public class EntityBaseMetadataCopyTests
{
    [Fact]
    public void CopyMetadataTo_CopiesEveryEntryIntoTheTarget()
    {
        var entity = new MetadataEntity();
        entity.SetMetadata("a", 1);
        entity.SetMetadata("b", "two");
        var target = new MetadataContainer();

        entity.CopyTo(target);

        Assert.Equal(1, target.GetValue<int>("a"));
        Assert.Equal("two", target.GetValue<string>("b"));
    }

    [Fact]
    public void GetMetadataContainer_ReturnsTheContainerBehindMetadata()
    {
        var entity = new MetadataEntity();

        entity.Container.SetValue("key", "value");

        Assert.Equal("value", entity.Metadata["key"]);
        Assert.Equal("value", entity.GetMetadata<string>("KEY"));
    }

    [Fact]
    public void SetProviderId_StoresTheProvider()
    {
        var entity = new ProviderEntity();

        entity.SetProviderId("github");

        Assert.Equal("github", entity.ProviderId);
    }

    [Fact]
    public void SetProviderId_Null_ThrowsArgumentNullException()
    {
        var entity = new ProviderEntity();

        var exception = Assert.Throws<ArgumentNullException>(() => entity.SetProviderId(null!));

        Assert.Equal("providerId", exception.ParamName);
    }

    [Fact]
    public void SetProviderSpecificId_StoresAndClearsTheIdentifier()
    {
        var entity = new ProviderEntity();

        entity.SetProviderSpecificId("sha-1");
        var stored = entity.ProviderSpecificId;
        entity.SetProviderSpecificId(null);

        Assert.Equal("sha-1", stored);
        Assert.Null(entity.ProviderSpecificId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void MetadataContainer_SetPropertyValue_BlankName_ThrowsArgumentException(string? name)
    {
        var container = new MetadataContainer();

        var exception = Assert.Throws<ArgumentException>(() => container.SetPropertyValue(name!, 1));

        Assert.Equal("propertyName", exception.ParamName);
    }

    [Fact]
    public void MetadataContainer_GetPropertyValue_ReturnsTheStoredValue()
    {
        var container = new MetadataContainer();
        container.SetPropertyValue("Order", 3);

        Assert.Equal(3, container.GetPropertyValue<int>("order"));
        Assert.Equal(9, container.GetPropertyValue("missing", 9));
    }

    private sealed class MetadataEntity : EntityBase<string>
    {
        public MetadataContainer Container => GetMetadataContainer();

        public void CopyTo(MetadataContainer target) => CopyMetadataTo(target);
    }

    private sealed class ProviderEntity : DomainEntity<string>
    {
    }
}
