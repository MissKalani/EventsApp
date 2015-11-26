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
        public void IsInvitedIsOnlyTrueWhenTheUserIsInvitedToAnEvent()
        {
            int id = 0;
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

                eventUoW.Save();
                id = e1.Id;
            }

            using (var context = new EventContext())
            {
                var eventUoW = new EventUnitOfWork(context);
                var userManager = new UserManager<AppUser>(new UserStore<AppUser>(context));
                var user1 = context.Users.Single(t => t.UserName == "TestUser");
                var user2 = context.Users.Single(t => t.UserName == "TestUser2");
                var user3 = context.Users.Single(t => t.UserName == "TestUser3");
                var e = eventUoW.Events.GetEventByID(id);

                eventUoW.Invites.IsInvited(e, user2).Should().BeTrue();
                eventUoW.Invites.IsInvited(e, user3).Should().BeFalse();
            }
        }

        [TestMethod]
        public void IsInvitedIsNotTrueForTheHost()
        {
            int id = 0;
            using (var context = new EventContext())
            {
                var eventUoW = new EventUnitOfWork(context);
                var userManager = new UserManager<AppUser>(new UserStore<AppUser>(context));
                var user1 = context.Users.Single(t => t.UserName == "TestUser");

                Event e1 = new Event();
                e1.Brief = "User 1 Event";
                e1.Visibility = EventVisibility.Private;
                e1.ModificationState = ModificationState.Added;
                e1.AppUser = user1;
                eventUoW.Events.Attach(e1);

                eventUoW.Save();
                id = e1.Id;
            }

            using (var context = new EventContext())
            {
                var eventUoW = new EventUnitOfWork(context);
                var userManager = new UserManager<AppUser>(new UserStore<AppUser>(context));
                var user1 = context.Users.Single(t => t.UserName == "TestUser");
                var e = eventUoW.Events.GetEventByID(id);

                eventUoW.Invites.IsInvited(e, user1).Should().BeFalse();
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

        [TestMethod]
        public void GetPendingInvitesWithEventGraphReturnsOnlyEventsTheUserIsInvitedTo()
        {
            using (var context = new EventContext())
            {
                var eventUoW = new EventUnitOfWork(context);
                var userManager = new UserManager<AppUser>(new UserStore<AppUser>(context));
                var user1 = context.Users.Single(t => t.UserName == "TestUser");
                var user2 = context.Users.Single(t => t.UserName == "TestUser2");
                var user3 = context.Users.Single(t => t.UserName == "TestUser3");

                Event e1 = new Event();
                e1.Brief = "User 1 Event 1";
                e1.Visibility = EventVisibility.Private;
                e1.ModificationState = ModificationState.Added;
                e1.AppUser = user1;
                eventUoW.Events.Attach(e1);

                Event e2 = new Event();
                e2.Brief = "User 1 Event 2";
                e2.Visibility = EventVisibility.Private;
                e2.ModificationState = ModificationState.Added;
                e2.AppUser = user1;
                eventUoW.Events.Attach(e2);

                Event e3 = new Event();
                e3.Brief = "User 2 Event";
                e3.Visibility = EventVisibility.Private;
                e3.ModificationState = ModificationState.Added;
                e3.AppUser = user2;
                eventUoW.Events.Attach(e3);

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

                Invite i3 = new Invite();
                i3.AppUser = user1;
                i3.Event = e2;
                i3.ModificationState = ModificationState.Added;
                eventUoW.Invites.Attach(i3);

                eventUoW.Save();
            }

            using (var context = new EventContext())
            {
                var eventUoW = new EventUnitOfWork(context);
                var userManager = new UserManager<AppUser>(new UserStore<AppUser>(context));
                var user1 = context.Users.Single(t => t.UserName == "TestUser");
                var user2 = context.Users.Single(t => t.UserName == "TestUser2");
                var user3 = context.Users.Single(t => t.UserName == "TestUser3");

                var invites = eventUoW.Invites.GetPendingInvitesWithEventGraph(user2);

                invites.Should().HaveCount(1);
                invites.Single().Event.Brief.Should().Be("User 1 Event 1");
            }
        }

        [TestMethod]
        public void GetPendingInvitesWithEventGraphReturnsOnlyEventsThatArePending()
        {
            using (var context = new EventContext())
            {
                var eventUoW = new EventUnitOfWork(context);
                var userManager = new UserManager<AppUser>(new UserStore<AppUser>(context));
                var user1 = context.Users.Single(t => t.UserName == "TestUser");
                var user2 = context.Users.Single(t => t.UserName == "TestUser2");

                Event e1 = new Event();
                e1.Brief = "User 1 Event 1";
                e1.Visibility = EventVisibility.Private;
                e1.ModificationState = ModificationState.Added;
                e1.AppUser = user1;
                eventUoW.Events.Attach(e1);

                Event e2 = new Event();
                e2.Brief = "User 1 Event 2";
                e2.Visibility = EventVisibility.Private;
                e2.ModificationState = ModificationState.Added;
                e2.AppUser = user1;
                eventUoW.Events.Attach(e2);

                Invite i1 = new Invite();
                i1.AppUser = user2;
                i1.Event = e1;
                i1.Status = InviteStatus.Pending;
                i1.ModificationState = ModificationState.Added;
                eventUoW.Invites.Attach(i1);

                Invite i2 = new Invite();
                i2.AppUser = user2;
                i2.Event = e2;
                i2.Status = InviteStatus.Accepted;
                i2.ModificationState = ModificationState.Added;
                eventUoW.Invites.Attach(i2);

                eventUoW.Save();
            }

            using (var context = new EventContext())
            {
                var eventUoW = new EventUnitOfWork(context);
                var userManager = new UserManager<AppUser>(new UserStore<AppUser>(context));
                var user1 = context.Users.Single(t => t.UserName == "TestUser");
                var user2 = context.Users.Single(t => t.UserName == "TestUser2");

                var invites = eventUoW.Invites.GetPendingInvitesWithEventGraph(user2);

                invites.Should().HaveCount(1);
                invites.Single().Event.Brief.Should().Be("User 1 Event 1");
            }
        }

        [TestMethod]
        public void GetUnseenPendingInvitesCountReturnsNewInvites()
        {
            using (var context = new EventContext())
            {
                var eventUoW = new EventUnitOfWork(context);
                var userManager = new UserManager<AppUser>(new UserStore<AppUser>(context));
                var user1 = context.Users.Single(t => t.UserName == "TestUser");
                var user2 = context.Users.Single(t => t.UserName == "TestUser2");

                Event e1 = new Event();
                e1.Brief = "User 1 Event 1";
                e1.Visibility = EventVisibility.Private;
                e1.ModificationState = ModificationState.Added;
                e1.AppUser = user1;
                eventUoW.Events.Attach(e1);

                Invite i1 = new Invite();
                i1.AppUser = user2;
                i1.Event = e1;
                i1.Status = InviteStatus.Pending;
                i1.ModificationState = ModificationState.Added;
                eventUoW.Invites.Attach(i1);

                eventUoW.Save();
            }

            using (var context = new EventContext())
            {
                var eventUoW = new EventUnitOfWork(context);
                var userManager = new UserManager<AppUser>(new UserStore<AppUser>(context));
                var user1 = context.Users.Single(t => t.UserName == "TestUser");
                var user2 = context.Users.Single(t => t.UserName == "TestUser2");

                var count = eventUoW.Invites.GetUnseenPendingInvitesCount(user2);

                count.Should().Be(1);
            }
        }

        [TestMethod]
        public void GetUnseenPendingInvitesCountReturnsOnlyUnseenInvites()
        {
            using (var context = new EventContext())
            {
                var eventUoW = new EventUnitOfWork(context);
                var userManager = new UserManager<AppUser>(new UserStore<AppUser>(context));
                var user1 = context.Users.Single(t => t.UserName == "TestUser");
                var user2 = context.Users.Single(t => t.UserName == "TestUser2");

                Event e1 = new Event();
                e1.Brief = "User 1 Event 1";
                e1.Visibility = EventVisibility.Private;
                e1.ModificationState = ModificationState.Added;
                e1.AppUser = user1;
                eventUoW.Events.Attach(e1);

                Event e2 = new Event();
                e2.Brief = "User 1 Event 2";
                e2.Visibility = EventVisibility.Private;
                e2.ModificationState = ModificationState.Added;
                e2.AppUser = user1;
                eventUoW.Events.Attach(e2);

                Invite i1 = new Invite();
                i1.AppUser = user2;
                i1.Event = e1;
                i1.Status = InviteStatus.Pending;
                i1.Seen = true;
                i1.ModificationState = ModificationState.Added;
                eventUoW.Invites.Attach(i1);

                Invite i2 = new Invite();
                i2.AppUser = user2;
                i2.Event = e2;
                i2.Status = InviteStatus.Pending;
                i2.ModificationState = ModificationState.Added;
                eventUoW.Invites.Attach(i2);

                eventUoW.Save();
            }

            using (var context = new EventContext())
            {
                var eventUoW = new EventUnitOfWork(context);
                var userManager = new UserManager<AppUser>(new UserStore<AppUser>(context));
                var user1 = context.Users.Single(t => t.UserName == "TestUser");
                var user2 = context.Users.Single(t => t.UserName == "TestUser2");

                var count = eventUoW.Invites.GetUnseenPendingInvitesCount(user2);

                count.Should().Be(1);
            }
        }

        [TestMethod]
        public void MarkAllInvitesAsSeenShouldResetTheUnseenInviteCount()
        {
            using (var context = new EventContext())
            {
                var eventUoW = new EventUnitOfWork(context);
                var userManager = new UserManager<AppUser>(new UserStore<AppUser>(context));
                var user1 = context.Users.Single(t => t.UserName == "TestUser");
                var user2 = context.Users.Single(t => t.UserName == "TestUser2");

                Event e1 = new Event();
                e1.Brief = "User 1 Event 1";
                e1.Visibility = EventVisibility.Private;
                e1.ModificationState = ModificationState.Added;
                e1.AppUser = user1;
                eventUoW.Events.Attach(e1);

                Event e2 = new Event();
                e2.Brief = "User 1 Event 2";
                e2.Visibility = EventVisibility.Private;
                e2.ModificationState = ModificationState.Added;
                e2.AppUser = user1;
                eventUoW.Events.Attach(e2);

                Invite i1 = new Invite();
                i1.AppUser = user2;
                i1.Event = e1;
                i1.Status = InviteStatus.Pending;
                i1.ModificationState = ModificationState.Added;
                eventUoW.Invites.Attach(i1);

                Invite i2 = new Invite();
                i2.AppUser = user2;
                i2.Event = e2;
                i2.Status = InviteStatus.Pending;
                i2.ModificationState = ModificationState.Added;
                eventUoW.Invites.Attach(i2);

                eventUoW.Save();
            }

            using (var context = new EventContext())
            {
                var eventUoW = new EventUnitOfWork(context);
                var userManager = new UserManager<AppUser>(new UserStore<AppUser>(context));
                var user1 = context.Users.Single(t => t.UserName == "TestUser");
                var user2 = context.Users.Single(t => t.UserName == "TestUser2");

                var count = eventUoW.Invites.GetUnseenPendingInvitesCount(user2);
                count.Should().Be(2);

                eventUoW.Invites.MarkAllInvitesAsSeen(user2);
                eventUoW.Save();
            }

            using (var context = new EventContext())
            {
                var eventUoW = new EventUnitOfWork(context);
                var userManager = new UserManager<AppUser>(new UserStore<AppUser>(context));
                var user1 = context.Users.Single(t => t.UserName == "TestUser");
                var user2 = context.Users.Single(t => t.UserName == "TestUser2");

                var count = eventUoW.Invites.GetUnseenPendingInvitesCount(user2);
                count.Should().Be(0);
            }
        }

        [TestMethod]
        public void TransferInviteOwnershipShouldTransferAllInvites()
        {
            using (var context = new EventContext())
            {
                var eventUoW = new EventUnitOfWork(context);
                var userManager = new UserManager<AppUser>(new UserStore<AppUser>(context));
                var user1 = context.Users.Single(t => t.UserName == "TestUser");
                var user2 = context.Users.Single(t => t.UserName == "TestUser2");
                var user3 = context.Users.Single(t => t.UserName == "TestUser3");

                Event e1 = new Event();
                e1.Brief = "User 1 Event 1";
                e1.Visibility = EventVisibility.Private;
                e1.ModificationState = ModificationState.Added;
                e1.AppUser = user1;
                eventUoW.Events.Attach(e1);

                Event e2 = new Event();
                e2.Brief = "User 1 Event 2";
                e2.Visibility = EventVisibility.Private;
                e2.ModificationState = ModificationState.Added;
                e2.AppUser = user1;
                eventUoW.Events.Attach(e2);

                Invite i1 = new Invite();
                i1.AppUser = user2;
                i1.Event = e1;
                i1.Status = InviteStatus.Pending;
                i1.ModificationState = ModificationState.Added;
                eventUoW.Invites.Attach(i1);

                Invite i2 = new Invite();
                i2.AppUser = user2;
                i2.Event = e2;
                i2.Status = InviteStatus.Pending;
                i2.ModificationState = ModificationState.Added;
                eventUoW.Invites.Attach(i2);

                eventUoW.Save();
            }

            using (var context = new EventContext())
            {
                var eventUoW = new EventUnitOfWork(context);
                var userManager = new UserManager<AppUser>(new UserStore<AppUser>(context));
                var user1 = context.Users.Single(t => t.UserName == "TestUser");
                var user2 = context.Users.Single(t => t.UserName == "TestUser2");
                var user3 = context.Users.Single(t => t.UserName == "TestUser3");

                eventUoW.Invites.TransferInviteOwnership(user2, user3);
                eventUoW.Save();
            }

            using (var context = new EventContext())
            {
                var eventUoW = new EventUnitOfWork(context);
                var userManager = new UserManager<AppUser>(new UserStore<AppUser>(context));
                var user1 = context.Users.Single(t => t.UserName == "TestUser");
                var user2 = context.Users.Single(t => t.UserName == "TestUser2");
                var user3 = context.Users.Single(t => t.UserName == "TestUser3");

                context.Invites.Where(t => t.AppUserId == user2.Id).ToList().Should().HaveCount(0);
                context.Invites.Where(t => t.AppUserId == user3.Id).ToList().Should().HaveCount(2);
            }
        }
    }
}
