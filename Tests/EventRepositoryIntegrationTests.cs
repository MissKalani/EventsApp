using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EventsApp.DataModels;
using FluentAssertions;

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

                Event e = new Event();
                e.Brief = "Test";
                e.Detailed = "Test";
                e.ModificationState = ModificationState.Added;

                repository.Attach(e);
            }

            using (var context = new EventContext())
            {
                Event e = context.Events.Where(t => t.Brief == "Test").Single();
                e.Detailed.Should().Be("Test");
            }
        }
    }
}
