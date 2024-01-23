namespace M47.Shared.Tests.Utils.Files;

using M47.Shared.Utils.Files;

public sealed class ProjectFilesTests
{
    [Fact]
    public void Should_ReturnCurrentProject_When_CallToGet()
    {
        // Arrange
        var expected = "Shared";

        // Act
        var actual = ProjectFiles.GetCurrentProjectName<ProjectFilesTests>();

        // Assert
        actual.Should().Be(expected);
    }
}