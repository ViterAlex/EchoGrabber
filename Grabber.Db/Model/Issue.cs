using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabber.Db.Model
{
    public class Issue : EchoGrabber.IssueInfo
    {
        public int Id { get; set; }
        public int PodcastId { get; set; }
    }
}
