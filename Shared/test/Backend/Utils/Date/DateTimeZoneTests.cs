namespace M47.Shared.Tests.Utils.Date;

using M47.Shared.Utils.Dates;

public sealed class DateTimeZoneTests
{
    [Fact]
    public void Should_ReturnDateFromTimeZone_When_Convert()
    {
        // Arrange
        var expected = new DateTime(1977, 08, 10).AddHours(2);
        var utcDate = new DateTime(1977, 08, 10);

        // Act
        var actualWindows = DateTimeZone.ConvertToZone(utcDate, "Romance Standard Time");
        var actualLinux = DateTimeZone.ConvertToZone(utcDate, "Europe/Madrid");

        // Assert
        actualWindows.Should().Be(expected);
        actualLinux.Should().Be(expected);
    }

    [Fact]
    public void Should_ReturnDateUTCTimeZone_When_ChangeTimezoneFromMadridTimezone()
    {
        // Arrange
        var expected = new DateTime(1977, 08, 10, 10, 0, 0);
        var localDate = new DateTime(1977, 08, 10, 12, 0, 0);

        const string sourceTimeZoneInfo = "Europe/Madrid";
        const string destinationTimeZoneInfo = "Etc/GMT";

        // Act
        var actual = DateTimeZone.ChangeTimezone(localDate, sourceTimeZoneInfo, destinationTimeZoneInfo);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Should_ReturnDefaultDateUTCTimeZone_When_ChangeTimezoneFromMadridTimezone()
    {
        // Arrange
        var expected = new DateTime(1977, 08, 10, 10, 0, 0);
        var localDate = new DateTime(1977, 08, 10, 12, 0, 0);

        const string sourceTimeZoneInfo = "Europe/Madrid";

        // Act
        var actual = DateTimeZone.ChangeTimezone(localDate, sourceTimeZoneInfo);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Should_ReturnDefaultDateUTCTimeZone_When_ChangeTimezoneFromIstanbulTimezone()
    {
        // Arrange
        var expected = new DateTime(1977, 08, 10, 13, 30, 0);
        var localDate = new DateTime(1977, 08, 10, 16, 30, 0);

        const string sourceTimeZoneInfo = "Europe/Istanbul";

        // Act
        var actual = DateTimeZone.ChangeTimezone(localDate, sourceTimeZoneInfo);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Should_ReturnDateMadridTimeZone_When_ChangeTimezoneFromIstanbulTimezone()
    {
        // Arrange
        var expected = new DateTime(1977, 08, 10, 12, 30, 0);
        var localDate = new DateTime(1977, 08, 10, 13, 30, 0);

        const string sourceTimeZoneInfo = "Europe/Istanbul";
        const string destinationTimeZoneInfo = "Europe/Madrid";

        // Act
        var actual = DateTimeZone.ChangeTimezone(localDate, sourceTimeZoneInfo, destinationTimeZoneInfo);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Should_ReturnDateMadridTimeZone_When_ChangeTimezoneFromIstanbulTimezoneinMarch()
    {
        // Arrange
        var expected = new DateTime(1977, 02, 10, 12, 30, 0);
        var localDate = new DateTime(1977, 02, 10, 13, 30, 0);

        const string sourceTimeZoneInfo = "Europe/Istanbul";
        const string destinationTimeZoneInfo = "Europe/Madrid";

        // Act
        var actual = DateTimeZone.ChangeTimezone(localDate, sourceTimeZoneInfo, destinationTimeZoneInfo);

        // Assert
        actual.Should().Be(expected);
    }
}