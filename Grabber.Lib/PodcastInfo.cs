using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoGrabber
{
    //Информация о подкасте
    public class PodcastInfo : Info
    {
        public List<IssueInfo> Issues { get; set; }

        public override string ToString()
        {
            var count = Issues == null ? 0 : Issues.Count;
            if (count > 0) return $"{Title} ({count})";
            return $"{Title}";
        }
    }
}
