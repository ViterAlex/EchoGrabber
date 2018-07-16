using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace EchoGrabber
{
    public static class Grabber
    {

        private static readonly Dictionary<string, string> Xpathes = new Dictionary<string, string>()
        {
            { "Url", ".//a[@class='download iblock']"},
            { "Title", ".//*[@class='title type2']"},
            { "DateTime", ".//*[@class='datetime']"},
            { "Size", ".//a[contains(@class,'download')]//span[@class='size']"},
            { "Duration", ".//a[contains(@class,'listen')]//span[@class='size']"},
            { "Base", "//div[contains(@class,'mainpreview')]//div[@class='prevcontent']"},
            { "NextPage", "//div[@class='pager']/a[@rel='next']"}
        };

        private static WebClient _client = new WebClient();

        public static IEnumerable<IssueInfo> GetAllLinks(string url)
        {
            url = $"https://echo.msk.ru{url}";
            var doc = GetDocument(url);

            var nodes = doc.DocumentNode.SelectNodes(Xpathes["Base"]).Select(n => GetIssueInfo(n));
            foreach (var node in nodes)
            {
                if (node.Title.IsNullOrEmpty())
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(url);
                    Console.ResetColor();
                }

                yield return node;
            }
            var nextPage = doc.DocumentNode.SelectSingleNode(Xpathes["NextPage"])?.Attributes["href"].Value;
            if (!string.IsNullOrEmpty(nextPage))
                foreach (var link in GetAllLinks(nextPage))
                    yield return link;
        }

        public static IssueInfo GetIssueInfo(HtmlNode node)
        {
            var url = node.SelectSingleNode(Xpathes["Url"])?.Attributes["href"].Value;
            var title = node.SelectSingleNode(Xpathes["Title"])?.InnerText;
            var dt = node.SelectSingleNode(Xpathes["DateTime"])?.Attributes["title"].Value;
            var duration = node.SelectSingleNode(Xpathes["Duration"])?.InnerText.Clean();
            var size = node.SelectSingleNode(Xpathes["Size"])?.InnerText.Clean();
            if (title.IsNullOrEmpty())
            {
                title = dt;
            }
            return new IssueInfo()
            {
                DateTime = dt,
                Title = title,
                Url = url,
                Duration = duration,
                Size = size
            };
        }

        public static string GetProgramName(string url)
        {
            url = $"https://echo.msk.ru{url}";
            var doc = GetDocument(url);
            return doc.DocumentNode.SelectSingleNode("//div[@class='conthead news']/h1")?.InnerText;
        }

        static private HtmlDocument GetDocument(string url)
        {
            using (var stream = new StreamReader(_client.OpenRead(url), Encoding.UTF8))
            {
                var doc = new HtmlDocument();
                doc.Load(stream);
                return doc;
            }
        }

        public static IEnumerable<IssueInfo> GetProgramLinks(string url = "/programs")
        {
            var doc = GetDocument($"https://echo.msk.ru{url}");
            var nodes = doc.DocumentNode.SelectNodes("//div[@class='title iblock']/h2/a")
                .Select(n => new IssueInfo()
                {
                    Url = n.Attributes["href"]?.Value,
                    Title = n.InnerText.ToSingleString().Trim()
                });
            return nodes;
        }

        public static void DownloadAll(string progTitle, string url)
        {
            throw new NotImplementedException();
        }
    }
}
