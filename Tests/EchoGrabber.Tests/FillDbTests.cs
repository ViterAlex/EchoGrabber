using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EchoGrabber.Tests
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

        [DataTestMethod]
        [DataRow(Grabber.ACTUAL)]
        [DataRow(Grabber.ARCHIVE)]
        public void LoadPodcastsTest(string url)
        {
            var g = new Db.Engine();
            g.LoadPodcasts(url);
        }


        [DataTestMethod]
        [DynamicData(nameof(GetPodcastId), DynamicDataSourceType.Method)]
        //[DataRow(1)]
        //[DataRow(2)]
        //[DataRow(3)]
        //[DataRow(4)]
        public void LoadIssues(int podcastId)
        {
            var g = new Db.Engine();
            g.LoadIssues(podcastId);
        }

        private static IEnumerable<object[]> GetPodcastId()
        {
            var g = new Db.Engine();
            var ids = new List<int>();
            using (var conn = g.OpenConnection())
            {
                var cmd = new SQLiteCommand(conn)
                {
                    CommandText = "SELECT Id" +
                                    "  FROM Podcasts" +
                                    " WHERE Id % 3 = 0 AND Archived = 1" +
                                    " LIMIT 20"
                };
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                    {
                        var id = Convert.ToInt32(reader["Id"]);
                        ids.Add(id);
                    }
            }
            foreach (var id in ids)
                yield return new object[]
                {
                    id
                };

        }
    }
}
