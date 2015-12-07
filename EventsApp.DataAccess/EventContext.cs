using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using EventsApp.DataModels;

namespace EventsApp.DataAccess
{
    public class EventContext : IdentityDbContext<AppUser>
    {
        public DbSet<Event> Events { get; set; }
        public DbSet<Invite> Invites { get; set; }
        public DbSet<InviteLink> InviteLinks { get; set; }

        public EventContext()
            : base("EventContext")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>().Property(t => t.Latitude).IsRequired();
            modelBuilder.Entity<Event>().Property(t => t.Longitude).IsRequired();
            modelBuilder.Entity<Event>().Property(t => t.StartTime).HasColumnType("datetime2");
            modelBuilder.Entity<Event>().Ignore(t => t.ModificationState);
            modelBuilder.Entity<Event>().HasRequired(t => t.AppUser).WithMany().WillCascadeOnDelete(true);

            modelBuilder.Entity<AppUser>().Ignore(t => t.ModificationState);
            modelBuilder.Entity<AppUser>().HasMany(t => t.Events).WithRequired(t => t.AppUser).HasForeignKey(t => t.OwnerId);
           

            modelBuilder.Entity<Invite>().HasKey(t => new { t.EventId, t.AppUserId });
            modelBuilder.Entity<Invite>().Property(t => t.Status).IsRequired();
            modelBuilder.Entity<Invite>().Property(t => t.Seen).IsRequired();
            modelBuilder.Entity<Invite>().Ignore(t => t.ModificationState);
            modelBuilder.Entity<Invite>().HasRequired(t => t.Event).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<Invite>().HasRequired(t => t.AppUser).WithMany().WillCascadeOnDelete(false);

            modelBuilder.Entity<InviteLink>().HasKey(t => t.LinkGUID);
            modelBuilder.Entity<InviteLink>().Property(t => t.OneTimeUse).IsRequired();
            modelBuilder.Entity<InviteLink>().Ignore(t => t.ModificationState);
            modelBuilder.Entity<InviteLink>().HasRequired(t => t.Event).WithMany().WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
        
    }
}
