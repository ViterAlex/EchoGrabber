using System;

namespace EchoGrabber
{
    //Информация о выпуске подкаста
    public class IssueInfo : Info
    {
        public DateTime DateTime { get; set; }
        public float Size { get; set; }
        public TimeSpan Duration { get; set; }
        public string Guests { get; set; }

        public override string ToString()
        {
            return $"Название: {Title}\r\n" +
                $@"Продолжительность: {Duration:hh\:mm\:ss}r\n" +
                $"Дата: {DateTime:dd.MM.YYYY HH:mm}\r\n" +
                $"Размер: {Size:f1} МБ\r\n" +
                $"Ссылка: {Url}\r\n";
        }
    }
}
