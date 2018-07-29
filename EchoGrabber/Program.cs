using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
namespace EchoGrabber
{
    class Program
    {
        //TODO:Получение ссылок из блога
        private const string ProgrammUrl = "/programs/brother/";

        static void Main(string[] args)
        {
            var url = args.Length > 0 ? args[0] : ProgrammUrl;
            var colors = (ConsoleColor[])Enum.GetValues(typeof(ConsoleColor));
            var l = colors.Length;
            var filename = $"{url.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[1]}.html";
            var progname = Grabber.GetProgramName(url);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(progname);
            Console.ResetColor();
            CreateHtml(filename, url);

            Process.Start(filename);
            Console.WriteLine();
        }

        private static void CreateHtml(string filename, string url)
        {
            var n = 0;
            string progName = Grabber.GetProgramName(url);
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
                    "}" +
                    "th{" +
                    "  background-color: #888888;" +
                    "  border-left: 2px solid;" +
                    "}" +
                    "td{" +
                    "  border-left: 2px solid;" +
                    "  padding: 10px;" +
                    "}" +
                    "tr{" +
                    "  border-top: 1px solid;" +
                    "}" +
                    "tr:nth-child(odd) {" +
                    "  background-color: #f2f2f2;" +
                    "}");
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

                foreach (var item in Grabber.GetAllPodcastLinks(url))
                {
                    if (item.Url.IsNullOrEmpty())
                    {
                        continue;
                    }
                    Console.Write($"\rПолучение ссылок: {++n}");
                    xmlWr.WriteStartElement("tr");

                    xmlWr.WriteStartElement("td");
                    xmlWr.WriteString(n.ToString().PadRight(10, '\u00A0'));
                    xmlWr.WriteEndElement();//td

                    xmlWr.WriteStartElement("td");

                    //xmlWr.WriteStartElement("p");
                    xmlWr.WriteStartElement("a");
                    xmlWr.WriteAttributeString("href", item.Url);
                    xmlWr.WriteAttributeString("title", $"Скачать подкаст за {item.DateTime}. ({item.Size.Trim()})");
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
                    xmlWr.WriteString(item.DateTime);
                    xmlWr.WriteEndElement();//td

                    xmlWr.WriteEndElement();//tr
                }
            }
        }
    }
}
