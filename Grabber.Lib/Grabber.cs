using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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

        public const string ARCHIVE = "/programs/archived";
        public const string ACTUAL = "/programs";

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
                DateTime = dt.ParseDateTime(),
                Title = title,
                Url = url,
                Duration = duration.ParseDuration(),
                Size = size.ParseSize(),
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
        public static IEnumerable<PodcastInfo> GetPodcastLinks(string url = ACTUAL)
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
                    var seconds = (int)Math.Truncate(issue.Duration.TotalSeconds);
                    stream.WriteLine($"#EXTINF:{seconds},«Эхо Москвы»,{++counter} {issue.Title}");
                    stream.WriteLine(issue.Url);
                }
            }
            return true;
        }

        public static async Task<bool> CreatePlaylistAsync(string filename, string url)
        {
            return await Task.Factory.StartNew(() => CreatePlaylist(filename, url));
        }

        public static void CreateHtml(string filename, string url)
        {
            var n = 0;
            string progName = GetProgramName(url);
            using (var stream = new StreamWriter(filename))
            using (var xmlWr = new XmlTextWriter(stream) { Formatting = Formatting.Indented })
            {
                xmlWr.WriteStartElement("html");
                xmlWr.WriteStartElement("head");

                xmlWr.WriteStartElement("meta");
                xmlWr.WriteAttributeString("charset", "utf-8");
                xmlWr.WriteEndElement();//meta

                xmlWr.WriteStartElement("meta");
                xmlWr.WriteAttributeString("name", "author");
                xmlWr.WriteAttributeString("content", "Oleksandr Viter, viter.alex@gmail.com");
                xmlWr.WriteEndElement();//meta

                xmlWr.WriteStartElement("title");
                xmlWr.WriteString($"Подкасты программы «{progName}»");
                xmlWr.WriteEndElement();//title

                xmlWr.WriteStartElement("style");
                xmlWr.WriteString(
                    "table{" +
                    "  border: 3px solid;" +
                    "  border-collapse: collapse;" +
                    "}\r\n" +
                    "th{" +
                    "  background-color: #888888;" +
                    "  border-left: 2px solid;" +
                    "}\r\n" +
                    "td{" +
                    "  border-left: 2px solid;" +
                    "  padding: 10px;" +
                    "}\r\n" +
                    "tr{" +
                    "  border-top: 1px solid;" +
                    "}\r\n" +
                    "tr:nth-child(odd) {" +
                    "  background-color: #f2f2f2;" +
                    "}\r\n");
                xmlWr.WriteEndElement();//style
                xmlWr.WriteEndElement(); //head

                xmlWr.WriteStartElement("body");
                xmlWr.WriteStartElement("h2");
                xmlWr.WriteStartElement("a");
                xmlWr.WriteAttributeString("href", $"https://echo.msk.ru{url}");
                xmlWr.WriteAttributeString("target", "_blank");
                xmlWr.WriteAttributeString("title", $"Перейти на страницу программы «{progName}»");
                xmlWr.WriteString(progName);
                xmlWr.WriteEndElement();//a
                xmlWr.WriteEndElement();//h2
                xmlWr.WriteStartElement("table");

                foreach (var item in GetAllPodcastLinks(url))
                {
                    if (item.Url.IsNullOrEmpty())
                    {
                        continue;
                    }
                    OnLinkProcessed(++n);
                    //Console.Write($"\rПолучение ссылок: {++n}");
                    xmlWr.WriteStartElement("tr");

                    xmlWr.WriteStartElement("td");
                    xmlWr.WriteString(n.ToString().PadRight(10, '\u00A0'));
                    xmlWr.WriteEndElement();//td

                    xmlWr.WriteStartElement("td");

                    //xmlWr.WriteStartElement("p");
                    xmlWr.WriteStartElement("a");
                    xmlWr.WriteAttributeString("href", item.Url);
                    xmlWr.WriteAttributeString("title", $"Скачать подкаст за {item.DateTime:dd.MM.YYYY}. ({item.Size} МБ)");
                    xmlWr.WriteString($"{item.Title} ({item.Duration})");
                    xmlWr.WriteEndElement();//a
                    //xmlWr.WriteEndElement();//p

                    if (!item.Guests.IsNullOrEmpty())
                    {
                        xmlWr.WriteRaw("<br>");
                        //xmlWr.WriteStartElement("p");
                        xmlWr.WriteString($"Гости: {item.Guests}");
                        //xmlWr.WriteEndElement();//p
                    }

                    xmlWr.WriteEndElement();//td

                    xmlWr.WriteStartElement("td");
                    xmlWr.WriteString(item.DateTime.ToShortDateString());
                    xmlWr.WriteEndElement();//td

                    xmlWr.WriteEndElement();//tr
                }
            }

        }
        public static async Task CreateHtmlAsync(string filename, string url)
        {
            await Task.Factory.StartNew(() => CreateHtml(filename, url));
        }

        public static event EventHandler<int> LinkProcessed;
        private static void OnLinkProcessed(int count)
        {
            LinkProcessed?.Invoke(null, count);
        }
    }
}
