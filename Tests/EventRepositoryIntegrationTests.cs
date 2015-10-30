using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EventsApp.DataModels;
using FluentAssertions;
using System.Security.Principal;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Tests
{
    [TestClass]
    public class EventRepositoryIntegrationTests
    {
        [TestMethod]
        public void CanCreateNewEvents()
        {
            using (var context = new EventContext())
            {
                var repository = new EventRepository(context);

                var userManager = new UserManager<AppUser>(new UserStore<AppUser>(context));
                AppUser user = new AppUser() { UserName = "TestUser" };
                userManager.Create(user, "TestPassword");

                Event e = new Event();
                e.Brief = "Test";
                e.Detailed = "Test";
                e.ModificationState = ModificationState.Added;
                e.AppUser = user;

                context.Events.Add(e);
                context.SaveChanges();

                //repository.Attach(e);
                //repository.Save();
            }

            using (var context = new EventContext())
            {
                Event e = context.Events.Single(t => t.Brief == "Test");
                e.Detailed.Should().Be("Test");
            }
        }
    }
}
