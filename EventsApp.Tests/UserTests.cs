using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EventsApp.DataAccess;
using FluentAssertions;
using Microsoft.AspNet.Identity;
using EventsApp.DataModels;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.Migrations;
using Tests;

namespace EventsApp.Tests
{
    /// <summary>
    /// Summary description for UserTests
    /// </summary>
    [TestClass]
    public class UserTests
    {       

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            // Since users are not cleaned up by transactions, we create the users we need for the test once.
            using (var context = new EventContext())
            {
                var userManager = new UserManager<AppUser>(new UserStore<AppUser>(context));
                AppUser user1 = new AppUser() { UserName = "Lars" };
                userManager.Create(user1, "TestPassword");

                AppUser user2 = new AppUser() { UserName = "Viktor" };
                userManager.Create(user2, "TestPassword");

                AppUser user3 = new AppUser() { UserName = "Harry" };
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
        public void should_return_username()
        {
            using( var context = new EventContext())
            {
                var eventUoW = new EventUnitOfWork(context);
                var user = eventUoW.Users.GetUserByUsername("Lars");

                user.Should().NotBeNull();
                user.UserName.Should().Be("Lars");
            }
            
        }

        [TestMethod]
        public void username_should_be_null()
        {
            using(var context = new EventContext())
            {
                var eventUoW = new EventUnitOfWork(context);
                var user = eventUoW.Users.GetUserByUsername("Laurene");

                user.Should().BeNull();

            }
        }
    }
}
