using System;
using System.Collections.Generic;
using eRxWeb.Controller;
using eRxWeb.ServerModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ProviderTests
{
    [TestClass]
    public class ProviderTests
    {
        [TestMethod]
        public void Should_return_Unique_Providers()
        {
            //Arrange
            var Providers = new List<SelectPatientProvider>();
            Providers.Insert(0, new SelectPatientProvider() { ProviderId = "30A65A0B-CD9A-4FD4-93CB-9916837F81E2", ProviderName = "Gokul Sibi" });
            Providers.Insert(1, new SelectPatientProvider() { ProviderId = "30A65A0B-CD9A-4FD4-93CB-9916837F81E2", ProviderName = "Gokul Sibi" });
            Providers.Insert(2, new SelectPatientProvider() { ProviderId = "46CFF857-F900-412D-8A7F-BEE2BCBAD1BF", ProviderName = "Ram Kumar" });
            Providers.Insert(3, new SelectPatientProvider() { ProviderId = "46CFF857-F900-412D-8A7F-BEE2BCBAD1BF", ProviderName = "Ram Kumar" });
            Providers.Insert(4, new SelectPatientProvider() { ProviderId = "719286E7-B0FB-49FE-A9F3-F6A704A5093F", ProviderName = "Raj Kumar" });
            Providers.Insert(5, new SelectPatientProvider() { ProviderId = "719286E7-B0FB-49FE-A9F3-F6A704A5093F", ProviderName = "Raj Kumar" });

            //Act
            var uniqueProviders = SelectPatientApiController.GetUniqueProviders(Providers);

            //Assert
            Assert.AreEqual(uniqueProviders.Count, 3);            

        }

    }
}
