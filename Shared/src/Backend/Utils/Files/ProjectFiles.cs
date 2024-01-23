namespace M47.Shared.Utils.Files;

using Castle.Core.Resource;
using System;
using System.Reflection;

public class ProjectFiles
{
    public static string GetAbsolutePath<T>()
    {
        var rootPath = AssemblyDirectory<T>();
        var projectPath = GetRelativePath<T>();

        return Path.Combine(Path.Join(rootPath, projectPath));
    }

    public static string FromSolutionPath<T>(string path)
    {
        var codeBase = GetRootPathOfSolution<T>();

        var solutionPath = Path.GetDirectoryName(Uri.UnescapeDataString(codeBase) + Uri.UnescapeDataString(path))!;

        return solutionPath;
    }

    public static string GetCurrentProjectName<T>()
    {
        var assembly = Assembly.GetAssembly(typeof(T))!.FullName!;
        var currentProject = assembly.Split(',')[0].Split('.')[1];

        return currentProject;
    }

    public static string AssemblyDirectory<T>()
    {
        var codeBase = typeof(T).Assembly.Location;
        var path = Uri.UnescapeDataString(codeBase);

        return Path.GetDirectoryName(path)!;
    }

    public static async Task<string> ReadResourceToEndAsync<T>(string pathFileName,
                                                               CancellationToken cancellationToken = default)
    {
        using var stream = OpenAsStream<T>(pathFileName);
        using var reader = new StreamReader(stream);

        return await reader.ReadToEndAsync(cancellationToken);
    }

    public static Stream OpenAsStream<T>(string pathFileName)
    {
        var assembly = Assembly.GetAssembly(typeof(T))!;
        var resource = GetResourceName<T>(pathFileName);

        var stream = assembly.GetManifestResourceStream(resource)!;

        if (stream is null)
        {
            throw new ResourceException($"The resource {pathFileName} not exists in the namespace {typeof(T).Namespace}.");
        }

        return stream;
    }

    private static string GetResourceName<T>(string pathFileName)
    {
        var @namespace = typeof(T).Namespace;
        var resourceName = pathFileName.Replace("/", ".")
                                       .Replace(@"\", ".")
                                       .TrimStart('.');

        return $"{@namespace}.{resourceName}";
    }

    private static string GetRelativePath<T>()
        => typeof(T).Namespace!.Replace($"{typeof(T).Assembly.GetName().Name}.", "")
                               .Replace($"{typeof(T).Assembly.GetName().Name}", "")
                               .Replace(".", "/");

    private static string GetRootPathOfSolution<T>()
    {
        var currentProject = GetCurrentProjectName<T>();

        var dir = Directory.GetParent(Directory.GetCurrentDirectory())!;

        while (Path.GetFileName(dir.FullName)
                   .Equals(currentProject, StringComparison.CurrentCultureIgnoreCase) == false)
        {
            dir = dir.Parent!;
        }

        return dir.Parent!.FullName;
    }
}