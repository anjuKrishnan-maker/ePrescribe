using System;
using System.Collections.Generic;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects;
using eRxWeb.Controller;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.ControllerTests.SettingsApiControllerTests
{
    [TestClass]
    public class EditUserLinkTests
    {
        [TestMethod]
        public void should_add_link()
        {
            //act
            var link = SettingsApiController.GenerateEditUserLink();

            //assert
            Assert.AreEqual("Edit Users", link.Label);
            Assert.AreEqual(Constants.PageNames.EDIT_USERS, link.ActionUrl);
        }
    }
}
