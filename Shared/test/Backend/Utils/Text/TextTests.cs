namespace M47.Shared.Tests.Utils.Text;

using M47.Shared.Utils.Text;

public sealed class TextTests : BaseTest
{
    [Theory]
    [InlineData("Lorem Ipsum is simply dummy text of the printing and typesetting industry.", 20, 20, "Lorem Ipsum is simply")]
    [InlineData("Lorem Ipsum; is simply.", 6, 15, "Lorem Ipsum")]
    [InlineData("LoremIpsum; is simply.", 5, 15, "LoremIpsum")]
    [InlineData("LoremIpsumissimplydummytext", 1, 4, "Lorem")]
    [InlineData("12341234 12341243/gsdfgsd", 10, 40, "12341234 12341243")]
    [InlineData("12341234 12341243.sdfgsdf", 10, 40, "12341234 12341243")]
    [InlineData("12341234 12341243\\sdfgsdf", 10, 40, "12341234 12341243")]
    [InlineData("12341234 12341243(sdfgsdf", 10, 40, "12341234 12341243")]
    [InlineData("12341234 12341243)sdfgsdf", 10, 40, "12341234 12341243")]
    [InlineData("12341234 12341243;sdfgsdf", 10, 40, "12341234 12341243")]
    [InlineData("12341234 12341243@sdfgsdf", 10, 40, "12341234 12341243")]
    [InlineData("12341234 12341243-sdfgsdf", 10, 40, "12341234 12341243")]
    [InlineData("12341234 12341243#sdfgsdf", 10, 40, "12341234 12341243")]
    [InlineData("12341234 12341243&sdfgsdf", 10, 40, "12341234 12341243")]
    [InlineData("Lorem Ipsum", 10000, 0, "Lorem Ipsum")]
    public void Should_TruncateTextByWord_When_TruncateByWord(string input, int length, int offset, string expected)
    {
        // Act
        var actual = Text.TruncateByWord(input, length: length, offset: offset);

        // Assert
        actual.Should().Be(expected);
    }
}