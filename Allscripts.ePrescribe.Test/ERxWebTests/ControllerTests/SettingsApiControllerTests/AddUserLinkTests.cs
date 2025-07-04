using System;
using System.Collections.Generic;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects;
using eRxWeb.AppCode;
using eRxWeb.Controller;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.ControllerTests.SettingsApiControllerTests
{
    [TestClass]
    public class AddUserLinkTests
    {
        [TestMethod]
        public void should_add_link()
        {
            //act
            var link = SettingsApiController.GenerateAddUserLink(EnabledDisabled.Enabled);

            //assert
            Assert.AreEqual("Add User", link.Label);
            Assert.AreEqual($"{Constants.PageNames.EDIT_USER}?Mode=Add", link.ActionUrl);
        }

        [TestMethod]
        public void should_not_add_link()
        {
            //act
            var link = SettingsApiController.GenerateAddUserLink(EnabledDisabled.Disabled);

            //assert
            Assert.IsNull(link);
        }
    }
}
