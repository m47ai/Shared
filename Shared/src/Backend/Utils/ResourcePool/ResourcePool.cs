namespace M47.Shared.Utils.ResourcePool;

using System.Collections.Concurrent;

public class ResourcePool<T> where T : class
{
    private readonly ConcurrentBag<T> _resources = new();
    private readonly Func<T> _factory;
    private int _count;

    public ResourcePool(Func<T> factory, int? poolSize = 1)
    {
        _factory = factory;
        _count = poolSize ?? Environment.ProcessorCount;

        Parallel.For(0, _count, i =>
        {
            _resources.Add(_factory());
        });
    }

    public T Acquire()
    {
        if (!_resources.TryTake(out T? resource))
        {
            resource = _factory();
            Interlocked.Increment(ref _count);
        }

        return resource;
    }

    public void Release(T resource)
    {
        _resources.Add(resource);
    }

    public int Count => _count;
}