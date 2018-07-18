﻿using HtmlAgilityPack;
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
            { "Url", ".//a[@class='download iblock']"},//Ссылка на mp3
            { "Title", ".//*[@class='title type2']"},//Название подкаста
            { "DateTime", ".//*[@class='datetime']"},//Дата выхода
            { "Size", ".//a[contains(@class,'download')]//span[@class='size']"},//Размер файла
            { "Duration", ".//a[contains(@class,'listen')]//span[@class='size']"},//Продолжительность
            { "Base", "//div[contains(@class,'mainpreview')]//div[@class='prevcontent']"},//Страница подкаста
            { "NextPage", "//div[@class='pager']/a[@rel='next']"}//Ссылка на следующую страницу с подкастами
        };

        private static WebClient _client = new WebClient();
        //Получение информации о выпуске
        private static IssueInfo GetIssueInfo(HtmlNode node)
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
        //Проверка, что на странице есть ссылки на подкасты
        private static bool ProgramHasPodcasts(string url)
        {
            url = $"https://echo.msk.ru{url}";
            var doc = GetDocument(url);

            var nodes = doc.DocumentNode.SelectNodes(Xpathes["Base"]);
            return nodes != null;
        }
        private static HtmlDocument GetDocument(string url)
        {
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
            var doc = GetDocument($"https://echo.msk.ru{url}");
            var nodes = doc.DocumentNode.SelectNodes("//div[@class='title iblock']/h2/a")
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
            return doc.DocumentNode.SelectSingleNode("//div[@class='conthead news']/h1")?.InnerText;
        }
        public static void DownloadAll(string progTitle, string url)
        {
            throw new NotImplementedException();
        }
        //Создание плейлиста
        public static void CreatePlaylist(string filename, string url)
        {
            using (var stream = new StreamWriter(filename))
            {
                stream.WriteLine("#EXTM3U");
                var counter = 1;
                foreach (var issue in GetAllPodcastLinks(url).Reverse())
                {
                    if (issue.Url.IsNullOrEmpty()) continue;
                    var format = issue.Duration.Length > 5 ? "h\\:mm\\:ss" : "mm\\:ss";
                    var ts = (int)Math.Truncate(TimeSpan.ParseExact(issue.Duration, format, CultureInfo.InvariantCulture).TotalSeconds);
                    stream.WriteLine($"#EXTINF:{ts},«Эхо Москвы»,{counter++} {issue.Title}");
                    stream.WriteLine(issue.Url);
                }
            }
        }
    }
}