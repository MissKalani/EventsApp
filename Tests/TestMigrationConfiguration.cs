using EventsApp.DataModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class TestMigrationConfiguration : DbMigrationsConfiguration<EventContext>
    {
        public TestMigrationConfiguration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsAssembly = Assembly.GetAssembly(typeof(EventContext));
            MigrationsNamespace = "EventsApp.DataModels.Migrations";
        }
    }
}
