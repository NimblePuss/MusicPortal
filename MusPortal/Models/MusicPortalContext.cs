using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MusPortal.Models
{
    public class MusicPortalContext: DbContext
    {
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<User> Users { get; set; }

        //http://www.entityframeworktutorial.net/code-first/configure-one-to-many-relationship-in-code-first.aspx
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //one-to-many 
            modelBuilder.Entity<Song>()
                        .HasRequired<Genre>(s => s.Genres) // Song entity requires Genre 
                        .WithMany(s => s.Songs); // Genre entity includes many Song entities
            base.OnModelCreating(modelBuilder);
        }

    }
}