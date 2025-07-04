using System;
using System.Collections.Generic;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects;
using eRxWeb.Controller;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.ControllerTests.SettingsApiControllerTests
{
    [TestClass]
    public class DurSettingsLinkTests
    {
        [TestMethod]
        public void should_add_link()
        {
            //act
            var link = SettingsApiController.GenerateDurSettingsLink(Visibility.Visible);

            //assert
            Assert.AreEqual("DUR Settings", link.Label);
            Assert.AreEqual(Constants.PageNames.USER_PREFERENCES, link.ActionUrl);
        }

        [TestMethod]
        public void should_not_add_link()
        {
            //act
            var link = SettingsApiController.GenerateDurSettingsLink(Visibility.Hidden);

            //assert
            Assert.IsNull(link);
        }
    }
}
