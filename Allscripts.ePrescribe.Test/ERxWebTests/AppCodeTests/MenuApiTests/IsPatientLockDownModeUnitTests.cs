using System;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.AppCodeTests.MenuApiTests
{
    [TestClass]
    public class IsPatientLockDownModeUnitTests
    {
        [TestMethod]
        public void should_return_false_when_ssoMode_is_not_set()
        {
            //arrange
            var expectedVal = false;

            //act
            bool isPatientLockDownMode = MenuApiHelper.IsPatientLockDownMode(string.Empty, "PATIENTID");

            //assert
            Assert.AreEqual(expectedVal, isPatientLockDownMode);
        }

        [TestMethod]
        public void should_return_false_when_patientId_is_not_passed()
        {
            //arrange
            var expectedVal = false;

            //act
            bool isPatientLockDownMode = MenuApiHelper.IsPatientLockDownMode(Constants.SSOMode.PATIENTLOCKDOWNMODE,string.Empty);

            //assert
            Assert.AreEqual(expectedVal, isPatientLockDownMode);
        }


    }
}
