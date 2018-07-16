namespace EchoGrabber
{
    public class IssueInfo
    {
        public string DateTime { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Size { get; set; }
        public string Duration { get; set; }

        public override string ToString()
        {
            return $"Название: {Title}\r\n" +
                $"Продолжительность: {Duration}r\n" +
                $"Дата: {DateTime}\r\n" +
                $"Размер: {Size}\r\n" +
                $"Ссылка: {Url}\r\n";
        }
    }
}
