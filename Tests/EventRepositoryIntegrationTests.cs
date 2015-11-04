using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EventsApp.DataModels;
using FluentAssertions;
using System.Security.Principal;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.Migrations;
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class EventRepositoryIntegrationTests : RollbackTestClass
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
        public void CanCreateNewEvents()
        {
            using (var context = new EventContext())
            {
                var repository = new EventRepository(context);
                var userManager = new UserManager<AppUser>(new UserStore<AppUser>(context));

                Event e = new Event();
                e.Brief = "Test";
                e.Detailed = "Test";
                e.ModificationState = ModificationState.Added;
                e.AppUser = context.Users.Single(t => t.UserName == "TestUser");

                repository.Attach(e);
                repository.Save();
            }

            using (var context = new EventContext())
            {
                Event e = context.Events.Single(t => t.Brief == "Test");
                e.Detailed.Should().Be("Test");
            }
        }

        [TestMethod]
        public void ChangingAnEventDisconnectedDoesNotCreateDuplicates()
        {
            Event e;
            using (var context = new EventContext())
            {
                var repository = new EventRepository(context);

                e = new Event();
                e.Brief = "Test";
                e.Detailed = "Test";
                e.ModificationState = ModificationState.Added;
                e.AppUser = context.Users.Single(t => t.UserName == "TestUser");

                repository.Attach(e);
                repository.Save();
            }

            e.Brief = "MyBrief";
            e.Detailed = "MyDetailed";
            e.ModificationState = ModificationState.Modified;

            using (var context = new EventContext())
            {
                var repository = new EventRepository(context);
                repository.Attach(e);
                repository.Save();
            }

            using (var context = new EventContext())
            {
                context.Invoking(t => t.Events.Single()).ShouldNotThrow();
            }
        }

        [TestMethod]
        public void GetAllPublicEventsOnlyReturnsPublic()
        {
            using (var context = new EventContext())
            {
                var repository = new EventRepository(context);
                var userManager = new UserManager<AppUser>(new UserStore<AppUser>(context));
                var user = context.Users.Single(t => t.UserName == "TestUser");

                Event pub = new Event();
                pub.Brief = "Public";
                pub.Visibility = EventVisibility.Public;
                pub.ModificationState = ModificationState.Added;
                pub.AppUser = user;

                Event pri = new Event();
                pri.Brief = "Private";
                pri.Visibility = EventVisibility.Private;
                pri.ModificationState = ModificationState.Added;
                pri.AppUser = user;

                repository.Attach(pub);
                repository.Save();
            }

            using (var context = new EventContext())
            {
                var repository = new EventRepository(context);

                List<Event> events = repository.GetAllPublicEvents();

                events.Should().HaveCount(1);
                events[0].Brief.Should().Be("Public");
            }
        }

        [TestMethod]
        public void GetAllInvitedEventsOnlyReturnsEventsThisUserIsInvitedTo()
        {
            using (var context = new EventContext())
            {
                var repository = new EventRepository(context);
                var userManager = new UserManager<AppUser>(new UserStore<AppUser>(context));
                var user1 = context.Users.Single(t => t.UserName == "TestUser");
                var user2 = context.Users.Single(t => t.UserName == "TestUser2");

                Event e1 = new Event();
                e1.Brief = "User 1 Event";
                e1.Visibility = EventVisibility.Private;
                e1.ModificationState = ModificationState.Added;
                e1.AppUser = user1;

                Event e2 = new Event();
                e2.Brief = "User 2 Event";
                e2.Visibility = EventVisibility.Private;
                e2.ModificationState = ModificationState.Added;
                e2.AppUser = user2;

                Event e3 = new Event();
                e3.Brief = "User 1 Public Event";
                e3.Visibility = EventVisibility.Public;
                e3.ModificationState = ModificationState.Added;
                e3.AppUser = user1;

                Invite i = new Invite();
                i.AppUser = user2;
                i.Event = e1;

                repository.Attach(e1);
                repository.Attach(e2);
                repository.Attach(e3);
                context.Invites.Add(i);
                repository.Save();
            }

            using (var context = new EventContext())
            {
                var repository = new EventRepository(context);
                var userManager = new UserManager<AppUser>(new UserStore<AppUser>(context));
                var user2 = context.Users.Single(t => t.UserName == "TestUser2");

                List<Event> events = repository.GetAllInvitedEvents(user2);

                events.Should().HaveCount(1);
                events[0].Brief.Should().Be("User 1 Event");
            }
        }

        [TestMethod]
        public void GetAllCreatedEventsOnlyReturnsEventsThisUserHasCreated()
        {
            using (var context = new EventContext())
            {
                var repository = new EventRepository(context);
                var userManager = new UserManager<AppUser>(new UserStore<AppUser>(context));
                var user1 = context.Users.Single(t => t.UserName == "TestUser");
                var user2 = context.Users.Single(t => t.UserName == "TestUser2");

                Event e1 = new Event();
                e1.Brief = "User 1 Event";
                e1.Visibility = EventVisibility.Private;
                e1.ModificationState = ModificationState.Added;
                e1.AppUser = user1;

                Event e2 = new Event();
                e2.Brief = "User 2 Event";
                e2.Visibility = EventVisibility.Private;
                e2.ModificationState = ModificationState.Added;
                e2.AppUser = user2;

                repository.Attach(e1);
                repository.Attach(e2);
                repository.Save();
            }

            using (var context = new EventContext())
            {
                var repository = new EventRepository(context);
                var userManager = new UserManager<AppUser>(new UserStore<AppUser>(context));
                var user1 = context.Users.Single(t => t.UserName == "TestUser");

                List<Event> events = repository.GetAllCreatedEvents(user1);

                events.Should().HaveCount(1);
                events[0].Brief.Should().Be("User 1 Event");
            }
        }
    }
}
