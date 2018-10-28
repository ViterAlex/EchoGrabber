using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace EchoGrabber
{
    public static class Grabber
    {
        //xpath для отдельных элементов страницы
        private static readonly Dictionary<string, string> Xpathes = new Dictionary<string, string>()
        {
            { "Base", "//div[contains(@class,'mainpreview')]//div[@class='prevcontent']"},//Страница подкаста
            { "DateTime", ".//*[@class='datetime']"},//Дата выхода
            { "Duration", ".//a[contains(@class,'listen')]//span[@class='size']"},//Продолжительность
            { "Guests", ".//p[@class='author type1']/a//strong[@class='name']"},//Гости передачи
            { "NextPage", "//div[@class='pager']/a[@rel='next']"},//Ссылка на следующую страницу с подкастами
            { "ProgName", "//div[@class='conthead news']/h1"},//Название передачи в списке программ
            { "ProgHref", ".//div[@class='title iblock']/h2/a"},//Ссылка на страницу передачи
            { "Size", ".//a[contains(@class,'download')]//span[@class='size']"},//Размер файла
            { "Title", ".//*[@class='title type2']"},//Название подкаста
            { "Url", ".//a[@class='download iblock']"},//Ссылка на mp3
        };

        private static WebClient _client = new WebClient();
        //Получение информации о выпуске
        private static IssueInfo GetIssueInfo(HtmlNode node)
        {
            var url = node.SelectSingleNode(Xpathes["Url"])?.Attributes["href"].Value;
            var title = node.SelectSingleNode(Xpathes["Title"])?.InnerText?.Clean();
            var dt = node.SelectSingleNode(Xpathes["DateTime"])?.Attributes["title"].Value;
            var duration = node.SelectSingleNode(Xpathes["Duration"])?.InnerText.Clean();
            var size = node.SelectSingleNode(Xpathes["Size"])?.InnerText.Clean();
            var guests = node.SelectSingleNode(Xpathes["Guests"])?.InnerText;
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
                Size = size,
                Guests = guests
            };
        }
        //Проверка, что на странице есть ссылки на подкасты
        private static bool ProgramHasPodcasts(string url)
        {
            url = $"https://echo.msk.ru{url}";
            var doc = GetDocument(url);
            if (doc == null) return false;
            var nodes = doc.DocumentNode.SelectNodes(Xpathes["Base"]);
            return nodes != null;
        }
        private static HtmlDocument GetDocument(string url)
        {
            if (!Helper.EchoIsOnline) return null;
            using (var stream = new StreamReader(_client.OpenRead(url), Encoding.UTF8))
            {
                var doc = new HtmlDocument();
                doc.Load(stream);
                return doc;
            }
        }

        //Получение ссылок на все подкасты
        public static IEnumerable<IssueInfo> GetAllPodcastLinks(string url)
        {
            url = $"https://echo.msk.ru{url}";
            var doc = GetDocument(url);
            if (doc == null) yield break;

            var nodes = doc.DocumentNode.SelectNodes(Xpathes["Base"])?.Select(n => GetIssueInfo(n));
            if (nodes == null) yield break;//на странице нет подкастов
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
                foreach (var link in GetAllPodcastLinks(nextPage))
                    yield return link;
        }
        //Получение ссылок на страницы отдельных передач
        public static IEnumerable<PodcastInfo> GetPodcastLinks(string url = "/programs")
        {
            url = $"https://echo.msk.ru{url}";
            var doc = GetDocument(url);
            if (doc == null) return Enumerable.Empty<PodcastInfo>();

            var nodes = doc.DocumentNode.SelectNodes(Xpathes["ProgHref"])
                //TODO:Зависает на получении ссылок. Отсеивание страниц, на которых нет подкастов.
                //Если селектор Where убрать, то всё работает.
                //.Where(n => ProgramHasPodcasts(n.Attributes["href"].Value))
                .Select(n => new PodcastInfo()
                {
                    Url = n.Attributes["href"]?.Value,
                    Title = n.InnerText.ToSingleString().Trim()
                });
            return nodes;
        }
        //Получение названия передачи
        public static string GetProgramName(string url)
        {
            url = $"https://echo.msk.ru{url}";
            var doc = GetDocument(url);
            if (doc == null) return string.Empty;
            return doc.DocumentNode.SelectSingleNode(Xpathes["ProgName"])?.InnerText;
        }
        public static void DownloadAll(string progTitle, string url)
        {
            throw new NotImplementedException();
        }
        //Создание плейлиста
        public static bool CreatePlaylist(string filename, string url)
        {
            var podcasts = GetAllPodcastLinks(url).Reverse();
            if (!podcasts.Any(p => !p.Url.IsNullOrEmpty()))
            {
                return false;
            }
            using (var stream = new StreamWriter(filename))
            {
                stream.WriteLine("#EXTM3U");
                var counter = 0;
                foreach (var issue in podcasts)
                {
                    if (issue.Url.IsNullOrEmpty()) continue;
                    var format = issue.Duration.Length > 5 ? "h\\:mm\\:ss" : "mm\\:ss";
                    var seconds = (int)Math.Truncate(TimeSpan.ParseExact(issue.Duration, format, CultureInfo.InvariantCulture).TotalSeconds);
                    stream.WriteLine($"#EXTINF:{seconds},«Эхо Москвы»,{++counter} {issue.Title}");
                    stream.WriteLine(issue.Url);
                }
            }
            return true;
        }
    }
}
