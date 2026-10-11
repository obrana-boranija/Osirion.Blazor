using System.Collections;
using Microsoft.Extensions.DependencyInjection;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.TestDoubles;

/// <summary>
/// A service collection that rejects the first registration of one service type, so a test can drive the code path
/// that handles a failed registration.
/// </summary>
internal sealed class FailingOnceServiceCollection(Type failingServiceType) : IServiceCollection
{
    private readonly List<ServiceDescriptor> _descriptors = [];
    private bool _failed;

    public ServiceDescriptor this[int index]
    {
        get => _descriptors[index];
        set => _descriptors[index] = value;
    }

    public int Count => _descriptors.Count;

    public bool IsReadOnly => false;

    public void Add(ServiceDescriptor item)
    {
        if (!_failed && item.ServiceType == failingServiceType)
        {
            _failed = true;
            throw new InvalidOperationException($"Registration of {failingServiceType.Name} rejected by the test.");
        }

        _descriptors.Add(item);
    }

    public void Clear() => _descriptors.Clear();

    public bool Contains(ServiceDescriptor item) => _descriptors.Contains(item);

    public void CopyTo(ServiceDescriptor[] array, int arrayIndex) => _descriptors.CopyTo(array, arrayIndex);

    public IEnumerator<ServiceDescriptor> GetEnumerator() => _descriptors.GetEnumerator();

    public int IndexOf(ServiceDescriptor item) => _descriptors.IndexOf(item);

    public void Insert(int index, ServiceDescriptor item) => _descriptors.Insert(index, item);

    public bool Remove(ServiceDescriptor item) => _descriptors.Remove(item);

    public void RemoveAt(int index) => _descriptors.RemoveAt(index);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
