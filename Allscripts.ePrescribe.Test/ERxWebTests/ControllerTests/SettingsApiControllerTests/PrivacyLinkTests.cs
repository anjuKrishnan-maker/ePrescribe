using System;
using System.Collections.Generic;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects;
using eRxWeb.Controller;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.ControllerTests.SettingsApiControllerTests
{
    [TestClass]
    public class PrivacyLinkTests
    {
        [TestMethod]
        public void should_add_link()
        {
            //act
            var link = SettingsApiController.GeneratePrivacyLink(null);

            //assert
            Assert.AreEqual("Privacy", link.Label);
            Assert.AreEqual(Constants.PageNames.PRIVACY_PATIENT_SEARCH, link.ActionUrl);
        }

        [TestMethod]
        public void should_not_add_link()
        {
            //act
            var link = SettingsApiController.GeneratePrivacyLink(Constants.SSOMode.UTILITYMODE);

            //assert
            Assert.IsNull(link);
        }
    }
}
