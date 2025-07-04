using System;
using System.Collections.Generic;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects;
using Allscripts.Impact;
using eRxWeb.Controller;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.ControllerTests.SettingsApiControllerTests
{
    [TestClass]
    public class PreBuildRxLinkTests
    {
        [TestMethod]
        public void should_add_link_if_prebuilt_enabled_show_prebuilt_and_deluxeStatus_on()
        {
            //arrange
            var license = new ApplicationLicense
            {
                LicenseDeluxeStatus = Constants.DeluxeFeatureStatus.On
            };

            //act
            var link = SettingsApiController.GeneratePreBuildRxLink(license, Visibility.Visible, EnabledDisabled.Enabled);

            //assert
            Assert.AreEqual("Manage Pre-Built Prescriptions", link.Label);
            Assert.AreEqual(Constants.PageNames.PRE_BUILT_PRESCRIPTIONS, link.ActionUrl);
        }

        [TestMethod]
        public void should_add_link_if_prebuilt_enabled_show_prebuilt_and_deluxeStatus_alwayson()
        {
            //arrange
            var license = new ApplicationLicense
            {
                LicenseDeluxeStatus = Constants.DeluxeFeatureStatus.AlwaysOn
            };

            //act
            var link = SettingsApiController.GeneratePreBuildRxLink(license, Visibility.Visible, EnabledDisabled.Enabled);

            //assert
            Assert.AreEqual("Manage Pre-Built Prescriptions", link.Label);
            Assert.AreEqual(Constants.PageNames.PRE_BUILT_PRESCRIPTIONS, link.ActionUrl);
        }

        [TestMethod]
        public void should_add_link_if_prebuilt_enabled_show_prebuilt_and_deluxeStatus_cancelled()
        {
            //arrange
            var license = new ApplicationLicense
            {
                LicenseDeluxeStatus = Constants.DeluxeFeatureStatus.Cancelled
            };

            //act
            var link = SettingsApiController.GeneratePreBuildRxLink(license, Visibility.Visible, EnabledDisabled.Enabled);

            //assert
            Assert.AreEqual("Manage Pre-Built Prescriptions", link.Label);
            Assert.AreEqual(Constants.PageNames.PRE_BUILT_PRESCRIPTIONS, link.ActionUrl);
        }

        [TestMethod]
        public void should_not_add_link_if_prebuilt_enabled_show_prebuilt_and_deluxeStatus_compulsaryBasic()
        {
            //arrange
            var license = new ApplicationLicense
            {
                LicenseDeluxeStatus = Constants.DeluxeFeatureStatus.CompulsoryBasic
            };

            //act
            var link = SettingsApiController.GeneratePreBuildRxLink(license, Visibility.Visible, EnabledDisabled.Enabled);

            //assert
            Assert.IsNull(link);
        }

        [TestMethod]
        public void should_not_add_link_if_prebuilt_not_enabled_show_prebuilt_and_deluxeStatus_cancelled()
        {
            //arrange
            var license = new ApplicationLicense
            {
                LicenseDeluxeStatus = Constants.DeluxeFeatureStatus.Cancelled
            };

            //act
            var link = SettingsApiController.GeneratePreBuildRxLink(license, Visibility.Hidden, EnabledDisabled.Enabled);

            //assert
            Assert.IsNull(link);
        }

        [TestMethod]
        public void should_not_add_link_if_prebuilt_enabled_show_prebuilt_false_and_deluxeStatus_cancelled()
        {
            //arrange
            var license = new ApplicationLicense
            {
                LicenseDeluxeStatus = Constants.DeluxeFeatureStatus.Cancelled
            };

            //act
            var link = SettingsApiController.GeneratePreBuildRxLink(license, Visibility.Visible, EnabledDisabled.Disabled);

            //assert
            Assert.IsNull(link);
        }
    }
}
