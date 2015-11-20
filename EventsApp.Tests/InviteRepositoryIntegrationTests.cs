using EventsApp.DataAccess;
using EventsApp.DataModels;
using FluentAssertions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests;

namespace EventsApp.Tests
{
    [TestClass]
    public class InviteRepositoryIntegrationTests : RollbackTestClass
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            // Since users are not cleaned up by transactions, we create the users we need for the test once.
            using (var context = new EventContext())
            {
                var userManager = new UserManager<AppUser>(new UserStore<AppUser>(context));
                AppUser user1 = new AppUser() { UserName = "TestUser" };
                userManager.Create(user1, "TestPassword");

                AppUser user2 = new AppUser() { UserName = "TestUser2" };
                userManager.Create(user2, "TestPassword");

                AppUser user3 = new AppUser() { UserName = "TestUser3" };
                userManager.Create(user3, "TestPassword");
            }
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            // Since users are not cleaned up by transactions (why is this?), we need to clear the state
            // after these tests have been run.
            using (var context = new EventContext())
            {
                context.Database.Delete();

                var configuration = new TestMigrationConfiguration();
                var migrator = new DbMigrator(configuration);
                migrator.Update();
            }
        }

        [TestMethod]
        public void GetInvitedToEventReturnsOnlyPeopleInvitedToAnEvent()
        {
            using (var context = new EventContext())
            {
                var eventUoW = new EventUnitOfWork(context);
                var userManager = new UserManager<AppUser>(new UserStore<AppUser>(context));
                var user1 = context.Users.Single(t => t.UserName == "TestUser");
                var user2 = context.Users.Single(t => t.UserName == "TestUser2");
                var user3 = context.Users.Single(t => t.UserName == "TestUser3");

                Event e1 = new Event();
                e1.Brief = "User 1 Event";
                e1.Visibility = EventVisibility.Private;
                e1.ModificationState = ModificationState.Added;
                e1.AppUser = user1;
                eventUoW.Events.Attach(e1);

                Invite i1 = new Invite();
                i1.AppUser = user2;
                i1.Event = e1;
                i1.ModificationState = ModificationState.Added;
                eventUoW.Invites.Attach(i1);

                Invite i2 = new Invite();
                i2.AppUser = user3;
                i2.Event = e1;
                i2.ModificationState = ModificationState.Added;
                eventUoW.Invites.Attach(i2);

                eventUoW.Save();
            }

            using (var context = new EventContext())
            {
                var eventUoW = new EventUnitOfWork(context);
                var userManager = new UserManager<AppUser>(new UserStore<AppUser>(context));
                var e1 = context.Events.Single(t => t.Brief == "User 1 Event");
                var user1 = context.Users.Single(t => t.UserName == "TestUser");
                var user2 = context.Users.Single(t => t.UserName == "TestUser2");
                var user3 = context.Users.Single(t => t.UserName == "TestUser3");

                var invited = eventUoW.Invites.GetInvitedToEvent(e1);
                invited.Should().HaveCount(2);
                invited.Any(t => t.AppUser == null).Should().BeFalse();
                invited.Any(t => t.AppUser == user1).Should().BeFalse();
                invited.Any(t => t.AppUser == user2).Should().BeTrue();
                invited.Any(t => t.AppUser == user3).Should().BeTrue();
            }
        }
    }
}
