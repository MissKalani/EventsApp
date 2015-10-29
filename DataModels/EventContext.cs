using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace EventsApp.DataModels
{
    public class EventContext : IdentityDbContext<AppUser>
    {
        public DbSet<Event> Events { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>().Property(t => t.Latitude).IsRequired();
            modelBuilder.Entity<Event>().Property(t => t.Longitude).IsRequired();
            modelBuilder.Entity<Event>().Property(t => t.StartTime).HasColumnType("datetime2");

            modelBuilder.Entity<AppUser>().HasMany(t => t.Events).WithRequired(t => t.AppUser).HasForeignKey(t => t.AppUserId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
