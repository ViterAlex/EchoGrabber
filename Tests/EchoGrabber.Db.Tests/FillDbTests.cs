using System;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EchoGrabber.DbTests
{
    [TestClass]
    public class DbTests
    {

        [TestMethod]
        public void OpenConnectionTest()
        {
            var g = new Db.Engine();
            var conn = g.OpenConnection();
            Assert.IsTrue(conn.State == ConnectionState.Open);
        }

        [TestMethod]
        public void FillPodcastsTest()
        {
            var g = new Db.Engine();
            g.LoadPodcasts(Grabber.ARCHIVE);
        }
        [TestMethod]
        public void LoadIssues()
        {
            var g = new Db.Engine();
            g.LoadAllIssues();
        }
    }
}
