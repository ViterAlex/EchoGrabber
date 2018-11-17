namespace EchoGrabber
{
    //Информация о выпуске подкаста
    public class IssueInfo : Info
    {
        public string DateTime { get; set; }
        public string Size { get; set; }
        public string Duration { get; set; }
        public string Author { get; set; }
        public string Guests { get; set; }

        public override string ToString()
        {
            return $"Название: {Title}\r\n" +
                $"Автор: {Author}r\n" +
                $"Продолжительность: {Duration}r\n" +
                $"Дата: {DateTime}\r\n" +
                $"Размер: {Size}\r\n" +
                $"Ссылка: {Url}\r\n";
        }
    }
}
