using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EventsApp.DataModels;
using FluentAssertions;
using System.Security.Principal;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.Migrations;

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
                AppUser user = new AppUser() { UserName = "TestUser" };
                userManager.Create(user, "TestPassword");
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
    }
}
