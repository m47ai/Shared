namespace M47.Shared.Tests.Utils.ResourcePool;

using M47.Shared.Utils.ResourcePool;

public sealed class ResourcePoolTests
{
    [Fact]
    public void Should_Acquire_When_ReturnsResource()
    {
        // Arrange
        var factory = () => new object();
        var pool = new ResourcePool<object>(factory, 1);

        // Act
        var resource = pool.Acquire();

        // Assert
        resource.Should().NotBeNull();
    }

    [Fact]
    public void Should_Release_When_AddsResourceToPool()
    {
        // Arrange
        var factory = () => new object();
        var pool = new ResourcePool<object>(factory, 1);
        var resource = pool.Acquire();

        // Act
        pool.Release(resource);

        // Assert
        pool.Count.Should().Be(1);
    }

    [Fact]
    public void Should_AcquireInsufficientResources_When_CreatesNewResource()
    {
        // Arrange
        var factory = () => new object();
        var pool = new ResourcePool<object>(factory, 1);

        // Act
        var resource1 = pool.Acquire();
        var resource2 = pool.Acquire();

        // Assert
        pool.Count.Should().Be(2);
        resource1.Should().NotBe(resource2);
    }
}