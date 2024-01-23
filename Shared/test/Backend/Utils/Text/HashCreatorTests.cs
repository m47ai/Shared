namespace M47.Shared.Tests.Utils.Text;

using M47.Shared.Utils.Text;

public sealed class HashCreatorTests
{
    [Theory]
    [InlineData("https://www.instagram.com/p/CVgZChWK6nZ?id=17910919661038879", "0ce1148f3c04a857beae0a8898f40ad3")]
    [InlineData("https://www.instagram.com/p/CPYsSacNfU9?id=17858112557543248", "8fced08e73e717a2a980be6e7f47891b")]
    [InlineData("https://www.instagram.com/p/CPXFIcItssd?id=17848566290578724", "56d049b96d1ef516128a96d0e2713714")]
    public void Should_HashMD5_When_StringProivided(string input, string expected)
    {
        // Act
        var actual = HashCreator.CreateMd5(input);

        // Assert
        actual.Should().Be(expected);
    }
}