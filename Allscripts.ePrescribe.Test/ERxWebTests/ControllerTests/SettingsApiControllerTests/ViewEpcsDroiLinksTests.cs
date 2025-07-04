using System;
using System.Collections.Generic;
using Allscripts.ePrescribe.Objects;
using Allscripts.Impact;
using eRxWeb.Controller;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.ControllerTests.SettingsApiControllerTests
{
    [TestClass]
    public class ViewEpcsDroiLinksTests
    {
        [TestMethod]
        public void should_add_link_if_epcsEnabled_and_epcs_purchased()
        {
            //arrange
            ConfigKeys.TestInitialize(new Dictionary<string, string>{{ "ShieldEpcsDROIUrl", "linklink"} });

            //act
            var link = SettingsApiController.GenerateViewEpcsDroiLinks(EnabledDisabled.Enabled, EnabledDisabled.Enabled);

            //assert
            Assert.AreEqual("View EPCS Daily Reports Of Incidents", link.Label);
            Assert.AreEqual("linklink", link.ActionUrl);
            Assert.AreEqual(LinkLaunchType.NewWindow, link.LaunchType);
        }

        [TestMethod]
        public void should_add_link_if_enterprise_is_epcsEnabled_and_license_is_not_epcs_purchased()
        {
            //arrange
            ConfigKeys.TestInitialize(new Dictionary<string, string> { { "ShieldEpcsDROIUrl", "linklink" } });

            //act
            var link = SettingsApiController.GenerateViewEpcsDroiLinks(EnabledDisabled.Enabled, EnabledDisabled.Disabled);

            //assert
            Assert.AreEqual("View EPCS Daily Reports Of Incidents", link.Label);
            Assert.AreEqual("linklink", link.ActionUrl);
            Assert.AreEqual(LinkLaunchType.NewWindow, link.LaunchType);
        }

        [TestMethod]
        public void should_add_link_if_enterprise_is_not_epcsEnabled_and_license_is_epcs_purchased()
        {
            //arrange
            ConfigKeys.TestInitialize(new Dictionary<string, string> { { "ShieldEpcsDROIUrl", "linklink" } });

            //act
            var link = SettingsApiController.GenerateViewEpcsDroiLinks(EnabledDisabled.Disabled, EnabledDisabled.Enabled);

            //assert
            Assert.AreEqual("View EPCS Daily Reports Of Incidents", link.Label);
            Assert.AreEqual("linklink", link.ActionUrl);
            Assert.AreEqual(LinkLaunchType.NewWindow, link.LaunchType);
        }

        [TestMethod]
        public void should_not_add_link_if_enterprise_is_not_epcsEnabled_and_epcs_purchased_false()
        {
            //arrange
            ConfigKeys.TestInitialize(new Dictionary<string, string> { { "ShieldEpcsDROIUrl", "linklink" } });

            //act
            var link = SettingsApiController.GenerateViewEpcsDroiLinks(EnabledDisabled.Disabled, EnabledDisabled.Disabled);

            //assert
            Assert.IsNull(link);
        }
    }
}
