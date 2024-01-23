namespace M47.Shared.Utils.Text;

using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public static class HtmlRemoveTags
{
    public static string Clean(string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            return string.Empty;
        }

        var document = new HtmlDocument();
        document.LoadHtml(HttpUtility.HtmlDecode(data));

        var acceptableTags = new string[] { string.Empty };

        document.DocumentNode.Descendants()
                             .Where(n => n.Name is "script" or "style")
                             .ToList()
                             .ForEach(n => n.Remove());

        var nodes = new Queue<HtmlNode>(document.DocumentNode.SelectNodes("./*|./text()"));

        while (nodes.Count > 0)
        {
            var node = nodes.Dequeue();
            var parentNode = node.ParentNode;

            if (!acceptableTags.Contains(node.Name) && node.Name != "#text")
            {
                var childNodes = node.SelectNodes("./*|./text()");

                if (childNodes is not null)
                {
                    foreach (var child in childNodes)
                    {
                        nodes.Enqueue(child);
                        parentNode.InsertBefore(child, node);
                    }
                }

                parentNode.RemoveChild(node);
            }
        }

        return HttpUtility.HtmlDecode(document.DocumentNode.InnerHtml);
    }
}