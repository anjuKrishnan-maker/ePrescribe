using eRxWeb.Controller;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;
using Allscripts.ePrescribe.Objects.Registrant;
using System;
using static Allscripts.ePrescribe.Common.Constants;
using eRxWeb.State;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.AppCodeTests.RegistrationTests
{
    [TestClass]
    public class RegistrationStatusProgressCheckerTests
    {
        private IStateContainer Session { get; set; }
        [TestInitialize]
        public void InitializeLogon()
        {
            HttpContext.Current = MockHelpers.FakeHttpContext();
            Session = new StateContainer(HttpContext.Current.Session);
        }
        [TestMethod]
        public void Should_return_empty_registrant_info_if_RegistrantInfo_key_not_in_session()
        {
            //arrange
            var defaultRegistrantContext = new RegistrantContext();
            //act
            var registratContext = new ProgressChecker().GetContext(false);
            //assert
            Assert.AreEqual(defaultRegistrantContext.IsUserCreated, registratContext.IsUserCreated);
            Assert.AreEqual(defaultRegistrantContext.IsLOA3StatusConfirmed, registratContext.IsLOA3StatusConfirmed);
        }
        [TestMethod]
        public void Should_return_registrant_info_with_usercreated_status_true_if_RegistrantInfo_in_session_has_registrantid_greater_than_0_AND_has_ShieldObjectId()
        {
            //arrange
            var registrantInfo = new RegistrantInfo { ShieldObjectId = Guid.NewGuid(), RegistrantId = 0 + 1 };
            HttpContext.Current.Session[SessionVariables.RegistrantInfo] = registrantInfo;
            //act
            var registratContext = new ProgressChecker().GetContext(false);
            //assert
            Assert.IsTrue(registratContext.IsUserCreated);
        }
        [TestMethod]
        public void Should_return_registrant_info_with_usercreated_status_false_if_RegistrantInfo_in_session_lack_ShieldObjectId_or_Registrantid()
        {
            //arrange
            var registrantInfo = new RegistrantInfo { RegistrantId = 0 + 1 };
            HttpContext.Current.Session[SessionVariables.RegistrantInfo] = registrantInfo;
            //act
            var registratContext = new ProgressChecker().GetContext(false);
            //assert
            Assert.IsFalse(registratContext.IsUserCreated);
            //arrange
            var registrantInfoNoRegistrantId = new RegistrantInfo { ShieldObjectId = Guid.NewGuid() };
            HttpContext.Current.Session[SessionVariables.RegistrantInfo] = registrantInfoNoRegistrantId;
            //act
            var registratContextNoRegId = new ProgressChecker().GetContext(false);
            //assert
            Assert.IsFalse(registratContextNoRegId.IsUserCreated);
        }
        [TestMethod]
        public void Should_return_registrant_info_with_loa_status_false_if_user_created_is_false()
        {
            //arrange
            var registrantInfo = new RegistrantInfo { RegistrantId = 0 + 1 };
            HttpContext.Current.Session[SessionVariables.RegistrantInfo] = registrantInfo;
            //act
            var registratContext = new ProgressChecker().GetContext(false);
            //assert
            Assert.IsFalse(registratContext.IsUserCreated);
            Assert.IsFalse(registratContext.IsLOA3StatusConfirmed);
        }
        [TestMethod]
        public void Should_return_registrant_info_with_loa_status_true_if_user_created_is_true_and_loa_status_in_session_3()
        {
            //arrange
            var registrantInfo = new RegistrantInfo { ShieldObjectId = Guid.NewGuid(), RegistrantId = 0 + 1, LevelOfAssurance = "3" };
            HttpContext.Current.Session[SessionVariables.RegistrantInfo] = registrantInfo;
            //act
            var registratContext = new ProgressChecker().GetContext(false);
            //assert
            Assert.IsTrue(registratContext.IsUserCreated);
            Assert.IsTrue(registratContext.IsLOA3StatusConfirmed);
        }

        [TestMethod]
        public void Should_return_registrant_info_with_IseRxUser_true_if_session_has_userId()
        {
            //arrange
            HttpContext.Current.Session[SessionVariables.UserId] = Guid.NewGuid();
            //act
            var registratContext = new ProgressChecker().GetContext(false);
            //assert
            Assert.IsTrue(registratContext.IseRxUser);
        }
    } 
}

