using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace EventsApp.DataModels
{
    public class EventContext : DbContext
    {
        public DbSet<Event> Events { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>().Property(t => t.Latitude).IsRequired();
            modelBuilder.Entity<Event>().Property(t => t.Longitude).IsRequired();
            modelBuilder.Entity<Event>().Property(t => t.StartTime).HasColumnType("datetime2");
        }
    }
}
