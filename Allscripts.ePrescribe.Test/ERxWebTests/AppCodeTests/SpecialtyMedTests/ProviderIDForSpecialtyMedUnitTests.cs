using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using eRxWeb.AppCode;
using System.Data;
using Allscripts.ePrescribe.Common;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.SpecialtyMedTests
{
    [TestClass]
    public class ProviderIDForSpecialtyMedUnitTests
    {
        [TestMethod]
        public void should_set_Provider_NPI_As_SpecialtyMed_ProviderID_For_Regular_Provider()
        {
            //Arrange
            string expectedVal = "10050378";

            //Act
            string actualVal1 = new SpecialtyMed().RetrieveProviderIDForSpecialtyMed(Constants.UserCategory.PROVIDER, "10050378", null);
            string actualVal2 = new SpecialtyMed().RetrieveProviderIDForSpecialtyMed(Constants.UserCategory.PHYSICIAN_ASSISTANT, "10050378", null);

            //Assert
            Assert.AreEqual(expectedVal, actualVal1);
            Assert.AreEqual(expectedVal, actualVal2);
        }

        [TestMethod]
        public void should_set_Delegate_Provider_NPI_As_SpecialtyMed_ProviderID_For_POB()
        {
            //Arrange
            string expectedVal = string.Empty;

            //Act
            string actualVal1 = new SpecialtyMed().RetrieveProviderIDForSpecialtyMed(Constants.UserCategory.POB_SUPER, "10050378", null);
            string actualVal2 = new SpecialtyMed().RetrieveProviderIDForSpecialtyMed(Constants.UserCategory.POB_REGULAR, "10050378", null);
            string actualVal3 = new SpecialtyMed().RetrieveProviderIDForSpecialtyMed(Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED, "10050378", null);
            string actualVal4 = new SpecialtyMed().RetrieveProviderIDForSpecialtyMed(Constants.UserCategory.POB_LIMITED, "10050378", null);

            //Assert
            Assert.AreEqual(expectedVal, actualVal1);
            Assert.AreEqual(expectedVal, actualVal2);
            Assert.AreEqual(expectedVal, actualVal3);
            Assert.AreEqual(expectedVal, actualVal4);

            //Other real value already done inside NPIFromDelegateProviderUnitTests
        }
    }
}
