using System;
using eRxWeb;
using eRxWeb.AppCode;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.SsoTests
{
    [TestClass]
    public class SetPartnerSessionTests
    {
        [TestMethod]
        public void should_set_page_state_bassed_off_partner_object_sent_in()
        {
            //arrange
            var pageStateMock = MockRepository.GenerateStub<IStateContainer>();

            var partnerId = "1234";
            var theme = "themeMickey";
            var logoutUrl = "google.com";

            var partner = new Partner
            {
                ID = partnerId,
                Theme = theme,
                LogoutUrl = logoutUrl
            };

            //act
            Sso.SetPartnerSession(partner, pageStateMock);

            //assert
            Assert.AreEqual(partnerId, pageStateMock["PartnerID"]);
            Assert.AreEqual(theme, pageStateMock["Theme"]);
            Assert.AreEqual(logoutUrl, pageStateMock["PartnerLogoutUrl"]);
        }

        [TestMethod]
        public void should_not_set_theme_if_it_is_empty()
        {
            //arrange
            var pageStateMock = MockRepository.GenerateStub<IStateContainer>();

            var partnerId = "1234";
            var theme = "";
            var logoutUrl = "google.com";

            var partner = new Partner
            {
                ID = partnerId,
                Theme = theme,
                LogoutUrl = logoutUrl
            };

            //act
            Sso.SetPartnerSession(partner, pageStateMock);

            //assert
            Assert.IsNull(pageStateMock["Theme"]);
        }

        [TestMethod]
        public void should_not_set_partner_logout_url_if_it_is_null()
        {
            //arrange
            var pageStateMock = MockRepository.GenerateStub<IStateContainer>();

            var partnerId = "1234";
            var theme = "asd";

            var partner = new Partner
            {
                ID = partnerId,
                Theme = theme,
                LogoutUrl = null
            };

            //act
            Sso.SetPartnerSession(partner, pageStateMock);

            //assert
            Assert.IsNull(pageStateMock["PartnerLogoutUrl"]);
        }
    }
}
