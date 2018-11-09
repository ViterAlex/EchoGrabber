using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using EchoGrabber;
using System.Data;

namespace EchoGrabber.Db
{
    public class Engine
    {
        public SQLiteConnection OpenConnection()
        {
            return new SQLiteConnection(@"Data Source='.\Db\grabber.db'").OpenAndReturn();
        }

        public void LoadPodcasts(string url)
        {
            var podcasts = Grabber.GetPodcastLinks(url);
            using (var conn = OpenConnection())
            {
                var cmd = new SQLiteCommand(conn)
                {
                    CommandText = "INSERT INTO Podcasts (Title,Url,Archived) VALUES (@title,@url,@archived)"
                };
                cmd.Parameters.AddWithValue("@title", "");
                cmd.Parameters.AddWithValue("@url", "");
                cmd.Parameters.AddWithValue("@archived", 1);
                //cmd.Parameters.AddWithValue("@archived", 0);
                foreach (var item in podcasts)
                {
                    cmd.Parameters["@title"].Value = item.Title;
                    cmd.Parameters["@url"].Value = item.Url;
                    cmd.ExecuteNonQuery();
                }

            }
        }

        public IEnumerable<PodcastInfo> GetPodcastInfos()
        {
            using (var conn = OpenConnection())
            {
                var cmd = new SQLiteCommand(conn)
                {
                    CommandText = "SELECT Title, Url, Archived FROM Podcasts"
                };
                var r = cmd.ExecuteReader();
                while (r.Read())
                {
                    yield return new PodcastInfo
                    {
                        Title = r.GetString(0),
                        Url = r.GetString(1),
                        IsArchived = r.GetInt32(2) != 0
                    };
                }
            }
        }

        public void LoadAllIssues()
        {
            var podcasts = GetPodcastInfos().ToList();
            using (var conn = OpenConnection())
            {
                var insertCmd = new SQLiteCommand(conn)
                {
                    CommandText = "INSERT OR IGNORE INTO Issues(Title,Url,DateTime,Duration,Guests,Size,PodcastId) " +
                    "VALUES (@title,@url,@datetime,@duration,@guests,@size," +
                    "(SELECT Id FROM Podcasts WHERE Url=@podcasturl AND Archived=@archived LIMIT 1))"
                };
                insertCmd.Parameters.Add("@title", DbType.String);
                insertCmd.Parameters.Add("@url", DbType.String);
                insertCmd.Parameters.Add("@datetime", DbType.String);
                insertCmd.Parameters.Add("@duration", DbType.String);
                insertCmd.Parameters.Add("@guests", DbType.String);
                insertCmd.Parameters.Add("@size", DbType.String);
                insertCmd.Parameters.Add("@podcasturl", DbType.String);
                insertCmd.Parameters.Add("@archived", DbType.Int32);
                foreach (var item in podcasts)
                {
                    insertCmd.Parameters["@podcasturl"].Value = item.Url;
                    insertCmd.Parameters["@archived"].Value = item.IsArchived ? 1 : 0;
                    var issues = Grabber.GetAllPodcastLinks(item.Url);
                    foreach (var issue in issues)
                    {
                        insertCmd.Parameters["@title"].Value = issue.Title;
                        insertCmd.Parameters["@url"].Value = issue.Url;
                        insertCmd.Parameters["@datetime"].Value = issue.DateTime;
                        insertCmd.Parameters["@duration"].Value = issue.Duration;
                        insertCmd.Parameters["@guests"].Value = issue.Guests;
                        insertCmd.Parameters["@size"].Value = issue.Size;
                        insertCmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public void LoadIssues()
        {
            using (var conn = OpenConnection())
            {
                var cmd = new SQLiteCommand(conn)
                {
                    CommandText = "SELECT Url FROM Podcasts WHERE Title=@title AND Archived=@archived"
                };
                const string podcastTitle = "Большое «Эхо»";
                cmd.Parameters.AddWithValue("@title", podcastTitle);
                cmd.Parameters.AddWithValue("@archived", 0);

                var insertCmd = new SQLiteCommand(conn)
                {
                    CommandText = "INSERT OR IGNORE INTO Issues(Title,Url,DateTime,Duration,Guests,Size,PodcastId) " +
                    "VALUES (@title,@url,@datetime,@duration,@guests,@size," +
                    "(SELECT Id FROM Podcasts WHERE Title=@podcasttitle AND Archived=@archived))"
                };
                insertCmd.Parameters.Add("@title", DbType.String);
                insertCmd.Parameters.Add("@url", DbType.String);
                insertCmd.Parameters.Add("@datetime", DbType.String);
                insertCmd.Parameters.Add("@duration", DbType.String);
                insertCmd.Parameters.Add("@guests", DbType.String);
                insertCmd.Parameters.Add("@size", DbType.String);
                insertCmd.Parameters.AddWithValue("@podcasttitle", podcastTitle);
                insertCmd.Parameters.AddWithValue("@archived", 0);

                var r = cmd.ExecuteReader();
                while (r.Read())
                {
                    var url = r.GetString(0);
                    var iss = Grabber.GetAllPodcastLinks(url);
                    foreach (var item in iss)
                    {
                        insertCmd.Parameters["@title"].Value = item.Title;
                        insertCmd.Parameters["@url"].Value = item.Url;
                        insertCmd.Parameters["@datetime"].Value = item.DateTime;
                        insertCmd.Parameters["@duration"].Value = item.Duration;
                        insertCmd.Parameters["@guests"].Value = item.Guests;
                        insertCmd.Parameters["@size"].Value = item.Size;
                        insertCmd.ExecuteNonQuery();
                        //insertCmd.Parameters["@podcast"].Value = item.Title;
                    }
                }
            }
        }
    }
}
