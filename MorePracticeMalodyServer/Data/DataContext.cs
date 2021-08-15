using Microsoft.EntityFrameworkCore;
using MorePracticeMalodyServer.Model.DbModel;

namespace MorePracticeMalodyServer.Data
{
    /// <summary>
    ///     Detailed data types of context Databases.
    /// </summary>
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Song> Songs { get; set; }
        public DbSet<Chart> Charts { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Download> Downloads { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventChart> EventCharts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Download>()
                .HasOne(d => d.Chart)
                .WithMany(c => c.Downloads);

            modelBuilder.Entity<Promotion>()
                .HasOne(p => p.Song);
        }
    }
}