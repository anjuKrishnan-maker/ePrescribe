using System;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.SsoTests
{
    [TestClass]
    public class GetTaskRedirectForEpaLockDownTests
    {
        [TestMethod]
        public void should_return_reports_page_if_usertype_is_general_user()
        {
            //arrange
            var pageState = MockRepository.GenerateStub<IStateContainer>();
            pageState.Stub(x => x.Cast("UserType", Constants.UserCategory.GENERAL_USER))
                .Return(Constants.UserCategory.GENERAL_USER);

            //act
            var result = Sso.GetTaskRedirectForEpaLockDown(pageState);

            //assert
            Assert.AreEqual(Constants.PageNames.REPORTS, result);
        }

        [TestMethod]
        public void should_return_reports_page_if_usertype_is_pob_limited()
        {
            //arrange
            var pageState = MockRepository.GenerateStub<IStateContainer>();
            pageState.Stub(x => x.Cast("UserType", Constants.UserCategory.GENERAL_USER))
                .Return(Constants.UserCategory.POB_LIMITED);

            //act
            var result = Sso.GetTaskRedirectForEpaLockDown(pageState);

            //assert
            Assert.AreEqual(Constants.PageNames.REPORTS, result);
        }

        [TestMethod]
        public void should_return_doc_refill_page_if_usertype_is_not_pob_limited_or_general_user()
        {
            //arrange
            var pageState = MockRepository.GenerateStub<IStateContainer>();
            pageState.Stub(x => x.Cast("UserType", Constants.UserCategory.GENERAL_USER))
                .Return(Constants.UserCategory.PHYSICIAN_ASSISTANT);

            //act
            var result = Sso.GetTaskRedirectForEpaLockDown(pageState);

            //assert
            Assert.AreEqual(Constants.PageNames.DOC_REFILL_MENU, result);
        }
    }
}
