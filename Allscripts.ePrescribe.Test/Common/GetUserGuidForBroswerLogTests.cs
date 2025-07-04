using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eRxWeb;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.Common
{
    [TestClass]
    public class UserGuidForBroswerLogTests
    {
        [TestMethod]
        public void should_get_user_guid_in_standalone_mode()
        {
            var userGuid = "37eeaba3-2f07-483f-8a42-7d35718d0019";
            var stateMock = MockRepository.GenerateMock<IStateContainer>();

            stateMock.Stub(s => s.GetStringOrEmpty("AuditLogUserLoginID")).Return(userGuid);

            var result = BasePage.GetUserGuidForBrowserLog(stateMock);
            
            Assert.AreEqual(userGuid, result);
        }

        [TestMethod]
        public void should_get_user_guid_in_sso_mode()
        {
            var userGuid = "37eeaba3-2f07-483f-8a42-7d35718d0019";
            var stateMock = MockRepository.GenerateMock<IStateContainer>();

            stateMock.Stub(s => s.GetStringOrEmpty("AuditLogUserLoginID")).Return(string.Empty);
            stateMock.Stub(s => s.GetStringOrEmpty("USERID")).Return(userGuid);

            var result = BasePage.GetUserGuidForBrowserLog(stateMock);

            Assert.AreEqual(userGuid, result);
        }

        [TestMethod]
        public void should_not_error_out_if_unknown_condition_detected()
        {
            var stateMock = MockRepository.GenerateMock<IStateContainer>();

            stateMock.Stub(s => s.GetStringOrEmpty("blah")).Return(string.Empty).IgnoreArguments();

            var result = BasePage.GetUserGuidForBrowserLog(stateMock);

            Assert.AreEqual(string.Empty, result);
        }
    }
}
