﻿using System.Net.Http;
using Equinor.ProCoSys.IPO.Command.Validators.RowVersionValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.IPO.WebApi.IntegrationTests

{
    [TestClass]
    public abstract class TestBase
    {
        protected static TestFactory TestFactory;
        private readonly RowVersionValidator _rowVersionValidator = new RowVersionValidator();

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext)
        {
            if (TestFactory == null)
            {
                TestFactory = new TestFactory();
            }
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            if (TestFactory != null)
            {
                TestFactory.Dispose();
                TestFactory = null;
            }
        }

        public HttpClient AnonymousClient(string plant) => TestFactory.GetClientForPlant(TestFactory.AnonymousUser, plant);
        public HttpClient SignerClient(string plant) => TestFactory.GetClientForPlant(TestFactory.SignerUser, plant);
        public HttpClient PlannerClient(string plant) => TestFactory.GetClientForPlant(TestFactory.PlannerUser, plant);
        public HttpClient ViewerClient(string plant) => TestFactory.GetClientForPlant(TestFactory.ViewerUser, plant);
        public HttpClient AuthenticatedHackerClient(string plant) => TestFactory.GetClientForPlant(TestFactory.HackerUser, plant);

        public void AssertRowVersionChange(string oldRowVersion, string newRowVersion)
        {
            Assert.IsTrue(_rowVersionValidator.IsValid(oldRowVersion));
            Assert.IsTrue(_rowVersionValidator.IsValid(newRowVersion));
            Assert.AreNotEqual(oldRowVersion, newRowVersion);
        }
    }
}
