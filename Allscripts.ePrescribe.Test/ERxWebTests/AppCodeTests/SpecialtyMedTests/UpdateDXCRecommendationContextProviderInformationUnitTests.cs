using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using eRxWeb.AppCode;
using eRxWeb.ePrescribeSvc;
using eRxWeb.AppCode.SpecialtyMedWorkflow;
using System.Data;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode.Interfaces;
using Allscripts.ePrescribe.Objects;
using Allscripts.ePrescribe.Data;
using Provider = Allscripts.Impact.Provider;
using RxUser = eRxWeb.ePrescribeSvc.RxUser;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.SpecialtyMedTests
{
    [TestClass]
    public class UpdateDXCRecommendationContextProviderInformationUnitTests
    {
        [TestMethod]
        public void should_update_with_empty_values_if_null_rxuser_supplied()
        {
            //Arrange
            DXCRecommendationContext context = new DXCRecommendationContext();
            SpecialtyMed specialtyMedsWorkflow = new SpecialtyMed();

            //Act
            specialtyMedsWorkflow.UpdateDXCRecommendationContextProviderInformation(string.Empty, string.Empty, null/*REAL TEST*/, string.Empty, ref context);


            //Assert --Checking one field is sufficient for scope of this test
            Assert.AreEqual(string.Empty, context.ProviderFirstName);
        }

        [TestMethod]
        public void should_update_with_empty_values_if_rxuser_supplied_has_null_values()
        {
            //Arrange
            DXCRecommendationContext context = new DXCRecommendationContext();
            SpecialtyMed specialtyMedsWorkflow = new SpecialtyMed();
            RxUser rxuser = new RxUser();

            //Act
            specialtyMedsWorkflow.UpdateDXCRecommendationContextProviderInformation(string.Empty, string.Empty, rxuser/*REAL TEST*/, string.Empty, ref context);


            //Assert --Checking one field is sufficient for scope of this test
            Assert.AreEqual(string.Empty, context.ProviderFirstName);
        }
    }
}
