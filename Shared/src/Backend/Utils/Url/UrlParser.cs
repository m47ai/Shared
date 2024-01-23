namespace M47.Shared.Utils.Url;

using M47.Shared.Utils.Files;
using Nager.PublicSuffix;
using System.IO;

public class UrlParser
{
    protected UrlParser()
    {
    }

    public static string GetMainDomain(string permalink)
    {
        //https://github.com/nager/Nager.PublicSuffix

        var currentDirectory = ProjectFiles.GetAbsolutePath<UrlParser>();

        var filePtah = Path.Join(currentDirectory, "effective_tld_names.dat");
        var domainParser = new DomainParser(new FileTldRuleProvider(filePtah));

        permalink = permalink.Replace("https://", "");
        permalink = permalink.Replace("http://", "");
        var domainInfo = domainParser.Parse(permalink);

        return $"{domainInfo.Domain}.{domainInfo.TLD}";
    }
}