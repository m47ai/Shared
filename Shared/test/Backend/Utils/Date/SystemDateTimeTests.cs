namespace M47.Shared.Tests.Utils.Date;

using M47.Shared.Utils.Dates;

public class SystemDateTimeTests
{
    private readonly Fixture _fixture;

    public SystemDateTimeTests()
    {
        _fixture = new();
    }

    [Fact]
    public void Should_ReturnCustomDateTime_When_CreateNewOne()
    {
        // Arrange
        var expected = _fixture.Create<DateTime>();
        SystemDateTime.Init(() => expected);

        // Act
        var actual = SystemDateTime.UtcNow;

        // Assert
        expected.Should().BeCloseTo(actual, TimeSpan.FromMilliseconds(10));
    }

    [Fact]
    public void Should_SystemDateTimeBeDifferentFromDateTime_When_InitsWithCustomDateTime()
    {
        // Arrange
        SystemDateTime.Init(() => DateTime.UtcNow.AddMonths(-1));
        var expected = DateTime.UtcNow;

        // Act
        var actual = SystemDateTime.UtcNow;

        // Assert
        expected.Should().NotBeCloseTo(actual, TimeSpan.FromHours(1));
    }

    [Fact]
    public void Should_SystemDateTimeBeEqualFromDateTime_When_UseDefaultInit()
    {
        // Arrange
        SystemDateTime.DefaultInit();
        var expected = DateTime.UtcNow;

        // Act
        var actual = SystemDateTime.UtcNow;

        // Assert
        expected.Should().BeCloseTo(actual, TimeSpan.FromMilliseconds(100));
    }
}