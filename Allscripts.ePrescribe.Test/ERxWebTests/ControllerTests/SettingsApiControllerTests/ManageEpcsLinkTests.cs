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
    public class ManageEpcsLinkTests
    {
        [TestMethod]
        public void should_add_link_if_epcsEnabled_and_epcs_purchased()
        {
            //act
            var link = SettingsApiController.GenerateManageEpcsLink(EnabledDisabled.Enabled, EnabledDisabled.Enabled);

            //assert
            Assert.AreEqual("Manage EPCS", link.Label);
            Assert.AreEqual(Constants.PageNames.EPCS_REGISTRATION + "?From=" + AngularStringUtil.CreateUrl(Constants.PageNames.SETTINGS), link.ActionUrl);
        }

        [TestMethod]
        public void should_add_link_if_enterprise_is_epcsEnabled_and_license_has_not_purchased_epcs()
        {
            //act
            var link = SettingsApiController.GenerateManageEpcsLink(EnabledDisabled.Enabled, EnabledDisabled.Disabled);

            //assert
            Assert.AreEqual("Manage EPCS", link.Label);
            Assert.AreEqual(Constants.PageNames.EPCS_REGISTRATION + "?From=" + AngularStringUtil.CreateUrl(Constants.PageNames.SETTINGS), link.ActionUrl);
        }

        [TestMethod]
        public void should_add_link_if_enterprise_is_not_epcsEnabled_and_license_has_purchased_epcs()
        {
            //act
            var link = SettingsApiController.GenerateManageEpcsLink(EnabledDisabled.Disabled, EnabledDisabled.Enabled);

            //assert
            Assert.AreEqual("Manage EPCS", link.Label);
            Assert.AreEqual(Constants.PageNames.EPCS_REGISTRATION + "?From=" + AngularStringUtil.CreateUrl(Constants.PageNames.SETTINGS), link.ActionUrl);
        }

        [TestMethod]
        public void should_not_add_link_if_enterprise_is_not_epcsEnabled_and_license_has_not_purchased_epcs()
        {
            //act
            var link = SettingsApiController.GenerateManageEpcsLink(EnabledDisabled.Disabled, EnabledDisabled.Disabled);

            //assert
            Assert.IsNull(link);
        }
    }
}
