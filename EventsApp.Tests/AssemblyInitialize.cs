using EventsApp.DataModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using EventsApp.DataAccess;

namespace Tests
{
    [TestClass]
    public class AssemblyInitialize
    {
        [AssemblyInitialize]
        public static void Initialize(TestContext testContext)
        {
            var context = new EventContext();
            context.Database.Delete();

            var configuration = new TestMigrationConfiguration();
            var migrator = new DbMigrator(configuration);
            migrator.Update();
        }

        [AssemblyCleanup]
        public static void Cleanup()
        {
            var context = new EventContext();
            context.Database.Delete();
        }
    }
}
