using Grabber.Db.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabber.Db
{
    public class AppContext : DbContext
    {
        public AppContext() : base("DefaultConnection")
        {

        }

        public DbSet<Podcast> Podcasts { get; set; }
        public DbSet<Issue> Issues { get; set; }
    }
}
