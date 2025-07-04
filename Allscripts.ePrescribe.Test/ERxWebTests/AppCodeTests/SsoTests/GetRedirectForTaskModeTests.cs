using System;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.SsoTests
{
    [TestClass]
    public class GetRedirectForTaskModeTests
    {
        private IStateContainer _sessionMock;

        [TestInitialize]
        public void init()
        {
            _sessionMock = MockRepository.GenerateMock<IStateContainer>();
        }
        [TestMethod]
        public void should_return_specific_url_if_edit_user_is_in_redirect_url()
        {
            //arrange
            var redirect = "EditUser";
            var taskUrl = "SsoIdProofing.aspx";

            //act
            var result = Sso.GetRedirect(redirect, taskUrl, false, new Sso(), _sessionMock);

            //assert
            Assert.AreEqual($"~/setheight.aspx?dest={Constants.PageNames.SPA_LANDING}?page={redirect}", result);
        }

        [TestMethod]
        public void should_return_specific_url_if_select_account_and_site_is_in_redirect_url()
        {
            //arrange
            var redirect = "SelectAccountAndSite.aspx";
            var taskUrl = "SsoIdProofing.aspx";
            var siteUrl = "LocalHost";

            var basePageMock = MockRepository.GenerateMock<IBasePage>();
            basePageMock.Stub(x => x.GetSiteUrl()).Return(siteUrl);
            //act
            var result = Sso.GetRedirect(redirect, taskUrl, false, basePageMock, _sessionMock);

            //assert
            Assert.AreEqual(siteUrl + "/" + Constants.PageNames.SELECT_ACCOUNT_AND_SITE + "?TargetURL=/" +
                        Constants.PageNames.SET_HEIGHT + "?dest=Spa.aspx?page=" + taskUrl, result);
        }

        [TestMethod]
        public void should_return_specific_url_if_neither_of_the_above_conditions_are_met()
        {
            //arrange
            var redirect = "notAPage.aspx";
            var taskUrl = "SsoIdProofing.aspx";
            var siteUrl = "LocalHost";
            string expectedResult = $"{siteUrl}/{Constants.PageNames.SET_HEIGHT}?dest={Constants.PageNames.SPA_LANDING}?page={taskUrl}";

            var basePageMock = MockRepository.GenerateMock<IBasePage>();
            basePageMock.Stub(x => x.GetSiteUrl()).Return(siteUrl);
            //act
            var result = Sso.GetRedirect(redirect, taskUrl, false, basePageMock, _sessionMock);

            //assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}
