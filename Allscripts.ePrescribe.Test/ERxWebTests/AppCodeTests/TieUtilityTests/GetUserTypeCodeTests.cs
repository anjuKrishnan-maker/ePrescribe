using System;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.TieUtilityTests
{
    [TestClass]
    public class GetUserTypeCodeTests
    {
        [TestMethod]
        public void should_return_provider_for_provider()
        {
            //arrange
            var userType = Constants.UserCategory.PROVIDER;

            //act
            var result = TieUtility.GetUserTypeCode(userType);

            //assert
            Assert.AreEqual("Provider", result);
        }

        [TestMethod]
        public void should_return_pa_for_physician_assistant()
        {
            //arrange
            var userType = Constants.UserCategory.PHYSICIAN_ASSISTANT;

            //act
            var result = TieUtility.GetUserTypeCode(userType);

            //assert
            Assert.AreEqual("PA", result);
        }

        [TestMethod]
        public void should_return_pa_for_physician_assistant_supervised()
        {
            //arrange
            var userType = Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED;

            //act
            var result = TieUtility.GetUserTypeCode(userType);

            //assert
            Assert.AreEqual("PA", result);
        }

        [TestMethod]
        public void should_return_pob_for_POB_SUPER()
        {
            //arrange
            var userType = Constants.UserCategory.POB_SUPER;

            //act
            var result = TieUtility.GetUserTypeCode(userType);

            //assert
            Assert.AreEqual("POB", result);
        }

        [TestMethod]
        public void should_return_pob_for_POB_REGULAR()
        {
            //arrange
            var userType = Constants.UserCategory.POB_REGULAR;

            //act
            var result = TieUtility.GetUserTypeCode(userType);

            //assert
            Assert.AreEqual("POB", result);
        }

        [TestMethod]
        public void should_return_pob_for_POB_LIMITED()
        {
            //arrange
            var userType = Constants.UserCategory.POB_LIMITED;

            //act
            var result = TieUtility.GetUserTypeCode(userType);

            //assert
            Assert.AreEqual("POB", result);
        }

        [TestMethod]
        public void should_return_general_for_GENERAL_USER()
        {
            //arrange
            var userType = Constants.UserCategory.GENERAL_USER;

            //act
            var result = TieUtility.GetUserTypeCode(userType);

            //assert
            Assert.AreEqual("General", result);
        }
    }
}
