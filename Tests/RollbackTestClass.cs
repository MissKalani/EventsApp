using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Tests
{
    /// <summary>
    /// Inheriting from this class will cause all tests to rollback changes when they finish running.
    /// </summary>
    [TestClass]
    public class RollbackTestClass
    {
        private TransactionScope transaction;

        [TestInitialize]
        public void TestInitialize()
        {
            transaction = new TransactionScope();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            transaction.Dispose();
        }
    }
}
