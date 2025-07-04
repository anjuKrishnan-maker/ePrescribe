using System;
using System.Collections.Generic;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects;
using eRxWeb.Controller;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.ControllerTests.SettingsApiControllerTests
{
    [TestClass]
    public class SiteManagementLinkTests
    {
        [TestMethod]
        public void should_add_link()
        {
            //act
            var link = SettingsApiController.GenerateSiteManagementLink();

            //assert
            Assert.AreEqual("Site Management", link.Label);
            Assert.AreEqual(Constants.PageNames.SITE_MANAGEMENT, link.ActionUrl);
        }
    }
}
