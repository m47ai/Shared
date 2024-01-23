namespace M47.Shared.Domain.CustomerEngagement;

public class AttachedFile
{
    public static AttachedFile Create(string name, string urlToDownload)
        => new(name, urlToDownload);

    public string Name { get; }

    public string UrlToDownload { get; }

    public AttachedFile(string name, string urlToDownload)
    {
        Name = name;
        UrlToDownload = urlToDownload;
    }
}