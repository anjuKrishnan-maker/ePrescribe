using System;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.SsoTests
{
    [TestClass]
    public class SetSsoModeTests
    {
        [TestMethod]
        public void should_set_patient_lock_down_mode_in_session_if_passed_in()
        {
            //arrange
            var pageState = MockRepository.GenerateStub<IStateContainer>();

            //act
            Sso.SetSsoMode(Constants.SSOMode.PATIENTLOCKDOWNMODE, pageState);

            //assert
            Assert.AreEqual(Constants.SSOMode.PATIENTLOCKDOWNMODE, pageState["SSOMode"]);
        }

        [TestMethod]
        public void should_set_task_mode_in_session_if_passed_in()
        {
            //arrange
            var pageState = MockRepository.GenerateStub<IStateContainer>();

            //act
            Sso.SetSsoMode(Constants.SSOMode.TASKMODE, pageState);

            //assert
            Assert.AreEqual(Constants.SSOMode.TASKMODE, pageState["SSOMode"]);
        }

        [TestMethod]
        public void should_set_epa_patient_lock_down_mode_in_session_if_passed_in()
        {
            //arrange
            var pageState = MockRepository.GenerateStub<IStateContainer>();

            //act
            Sso.SetSsoMode(Constants.SSOMode.EPAPATIENTLOCKDOWNTASKMODE, pageState);

            //assert
            Assert.AreEqual(Constants.SSOMode.EPAPATIENTLOCKDOWNTASKMODE, pageState["SSOMode"]);
        }

        [TestMethod]
        public void should_set_sso_idproofing_mode_in_session_if_passed_in()
        {
            //arrange
            var pageState = MockRepository.GenerateStub<IStateContainer>();

            //act
            Sso.SetSsoMode(Constants.SSOMode.SSOIDPROOFINGMODE, pageState);

            //assert
            Assert.AreEqual(Constants.SSOMode.SSOIDPROOFINGMODE, pageState["SSOMode"]);
        }

        [TestMethod]
        public void should_not_set_sso_mode_in_session_if_mode_is_null_or_empty()
        {
            //arrange
            var pageState = MockRepository.GenerateStub<IStateContainer>();

            //act
            Sso.SetSsoMode(null, pageState);

            Assert.AreEqual(null, pageState["SSOMode"]);


            Sso.SetSsoMode(string.Empty, pageState);

            Assert.AreEqual(null, pageState["SSOMode"]);
        }
    }
}
