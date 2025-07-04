using Allscripts.ePrescribe.Common;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.EPCSRegistrationPage
{
    [TestClass]
    public class RedirectForNonAdminTests
    {
        [TestMethod]
        public void should_redirect_to_default_if_request_is_from_EPCSRegistration_page()
        {
            //arrange
            string requestUrl = "EPCSRegistration.aspx";

            //act
            var redirect = eRxWeb.EPCSRegistration.GetRedirectForNonAdmin(requestUrl);

            //assert
            Assert.IsTrue(Constants.PageNames.SELECT_PATIENT.Contains(redirect));
        }

        [TestMethod]
        public void should_redirect_to_previous_page_if_request_is_not_from_EPCSRegistration_page()
        {
            //arrange
            string requestUrl = "EditUser";

            //act
            var redirect = eRxWeb.EPCSRegistration.GetRedirectForNonAdmin(requestUrl);

            //assert
            Assert.IsTrue(Constants.PageNames.EDIT_USER.Contains(redirect));
        }
    }
}
