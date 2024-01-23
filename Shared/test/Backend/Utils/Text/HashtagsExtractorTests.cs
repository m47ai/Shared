namespace M47.Shared.Tests.Utils.Text;

using M47.Shared.Utils.Text;

public class HashtagsExtractorTests : BaseTest
{
    [Fact]
    public void Should_ExtractHashtag_When_HasInText()
    {
        // Arrange
        var expected = "hashtag";

        // Act
        var actual = HashtagsExtractor.HashtagsInText($"List of hashtags #{expected}");

        // Assert
        actual.Should().HaveCount(1);
        actual.Should().Contain(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData(default(string?))]
    public void Should_ReturnDefaultList_When_HasEmptyOrNullText(string? expected)
    {
        // Act
        var actual = HashtagsExtractor.HashtagsInText(expected);

        // Assert
        actual.Should().HaveCount(0);
    }

    [Fact]
    public void Should_EmptyHashtag_When_HasInText()
    {
        // Act
        var actual = HashtagsExtractor.HashtagsInText("List of hashtags");

        // Asert
        actual.Should().BeEmpty();
    }
}