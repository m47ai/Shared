namespace M47.Shared.Tests.Utils.Sample;

using M47.Shared.Utils.Files;

public static class SampleFile
{
    public static async Task<string> ReadAsStringAsync<T>(string fileName, string folder = "Sample")
        => await ProjectFiles.ReadResourceToEndAsync<T>($"{folder}/{fileName}");

    public static Stream OpenAsStream<T>(string fileName, string folder = "Sample")
        => ProjectFiles.OpenAsStream<T>($"{folder}/{fileName}");
}