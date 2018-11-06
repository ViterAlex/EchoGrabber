using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using Grabber.Db.Model;

namespace Grabber.Db.Tests
{
    [TestClass]
    public class GrabberDbTests
    {
        [TestMethod]
        public void AddPodcastTest()
        {
            var db = new AppContext();
            db.Podcasts.Load();
            foreach (var item in EchoGrabber.Grabber.GetPodcastLinks())
            {
                var pi = new Podcast() { Title = item.Title, Url = item.Url };
                db.Podcasts.Add(pi);
            }
            db.SaveChanges();
        }

        [TestMethod]
        public void AddIssuesTest()
        {
            var db = new AppContext();
            db.Podcasts.Load();
            //var item = db.Podcasts.FirstAsync().Result;
            foreach (var item in db.Podcasts)
            {
                Console.WriteLine(item.Title);
                var iss = new Issue();
                foreach (var ii in EchoGrabber.Grabber.GetAllPodcastLinks(item.Url))
                {
                    iss = new Issue
                    {
                        DateTime = ii.DateTime,
                        Duration = ii.Duration,
                        Guests = ii.Guests,
                        Size = ii.Size,
                        Title = ii.Title,
                        Url = ii.Url,
                        PodcastId = item.Id
                    };
                    db.Issues.Add(iss);
                    Console.Write("\r{0}{1}", db.Issues.CountAsync().Result, iss.Title);
                }
                db.SaveChanges();
                Console.WriteLine();
            }
        }

    }
}
