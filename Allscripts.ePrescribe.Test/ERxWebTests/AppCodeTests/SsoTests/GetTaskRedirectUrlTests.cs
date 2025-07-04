using System;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.SsoTests
{
    [TestClass]
    public class GetTaskRedirectUrlTests
    {
        [TestMethod]
        public void should_return_doc_refill_menu_if_isprovider_is_true()
        {
            //arrange
            var pageState = MockRepository.GenerateStub<IStateContainer>();
            pageState.Stub(x => x.GetBooleanOrFalse("IsProvider")).Return(true);

            //act
            var result = Sso.GetTaskRedirectUrl(pageState);

            //assert
            Assert.AreEqual(Constants.PageNames.DOC_REFILL_MENU, result);
        }

        [TestMethod]
        public void should_return_doc_refill_menu_if_IsPA_is_true()
        {
            //arrange
            var pageState = MockRepository.GenerateStub<IStateContainer>();
            pageState.Stub(x => x.GetBooleanOrFalse("IsPA")).Return(true);

            //act
            var result = Sso.GetTaskRedirectUrl(pageState);

            //assert
            Assert.AreEqual(Constants.PageNames.DOC_REFILL_MENU, result);
        }

        [TestMethod]
        public void should_not_return_doc_refill_menu_if_IsPA_is_false_and_isprovider_is_false()
        {
            //arrange
            var pageState = MockRepository.GenerateStub<IStateContainer>();
            pageState.Stub(x => x.GetBooleanOrFalse("IsPA")).Return(false);
            pageState.Stub(x => x.GetBooleanOrFalse("IsProvider")).Return(false);

            //act
            var result = Sso.GetTaskRedirectUrl(pageState);

            //assert
            Assert.AreNotEqual(Constants.PageNames.DOC_REFILL_MENU, result);
        }

        [TestMethod]
        public void should_return_reports_if_user_type_is_generaluser()
        {
            //arrange
            var pageState = MockRepository.GenerateStub<IStateContainer>();
            pageState.Stub(x => x.Cast("UserType", Constants.UserCategory.GENERAL_USER))
                .Return(Constants.UserCategory.GENERAL_USER);

            //act
            var result = Sso.GetTaskRedirectUrl(pageState);

            //assert
            Assert.AreEqual(Constants.PageNames.REPORTS, result);
        }

        [TestMethod]
        public void should_return_reports_if_user_type_is_pob_limited()
        {
            //arrange
            var pageState = MockRepository.GenerateStub<IStateContainer>();
            pageState.Stub(x => x.Cast("UserType", Constants.UserCategory.GENERAL_USER))
                .Return(Constants.UserCategory.POB_LIMITED);

            //act
            var result = Sso.GetTaskRedirectUrl(pageState);

            //assert
            Assert.AreEqual(Constants.PageNames.REPORTS, result);
        }

        [TestMethod]
        public void should_not_return_reports_if_user_type_is_not_pob_limited_or_general_user()
        {
            //arrange
            var pageState = MockRepository.GenerateStub<IStateContainer>();
            pageState.Stub(x => x.Cast("UserType", Constants.UserCategory.GENERAL_USER))
                .Return(Constants.UserCategory.PHYSICIAN_ASSISTANT);

            //act
            var result = Sso.GetTaskRedirectUrl(pageState);

            //assert
            Assert.AreNotEqual(Constants.PageNames.REPORTS, result);
        }

        [TestMethod]
        public void should_return_listSendScripts_if_no_conditions_are_met()
        {
            //arrange
            var pageState = MockRepository.GenerateStub<IStateContainer>();
            pageState.Stub(x => x.Cast("UserType", Constants.UserCategory.GENERAL_USER))
                .Return(Constants.UserCategory.PHYSICIAN_ASSISTANT);

            //act
            var result = Sso.GetTaskRedirectUrl(pageState);

            //assert
            Assert.AreEqual(Constants.PageNames.LIST_SEND_SCRIPTS, result);
        }
    }
}
