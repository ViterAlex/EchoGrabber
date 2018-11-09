using System.Collections.Generic;

namespace EchoGrabber
{
    //Информация о подкасте
    public class PodcastInfo : Info
    {
        public List<IssueInfo> Issues { get; set; }
        public bool IsArchived { get; set; }
        public override string ToString()
        {
            var count = Issues == null ? 0 : Issues.Count;
            if (count > 0) return $"{Title} ({count})";
            return $"{Title}";
        }
    }
}
