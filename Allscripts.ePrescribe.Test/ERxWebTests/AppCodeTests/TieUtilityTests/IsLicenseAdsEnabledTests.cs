using System;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.TieUtilityTests
{
    [TestClass]
    public class IsLicenseAdsEnabledTests
    {
        [TestMethod]
        public void should_return_true_if_license_is_basic()
        {
            //arrange
            var license = new ApplicationLicense { DeluxePricingStructure = Constants.DeluxePricingStructure.Basic};

            //act
            var result = TieUtility.IsLicenseAdsEnabled(license);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_true_if_license_is_deluxeEpcsLogRx()
        {
            //arrange
            var license = new ApplicationLicense { DeluxePricingStructure = Constants.DeluxePricingStructure.DeluxeEpcsLogRx };

            //act
            var result = TieUtility.IsLicenseAdsEnabled(license);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_true_if_license_is_DeluxeEpaLogRx()
        {
            //arrange
            var license = new ApplicationLicense { DeluxePricingStructure = Constants.DeluxePricingStructure.DeluxeEpaLogRx };

            //act
            var result = TieUtility.IsLicenseAdsEnabled(license);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_true_if_license_is_DeluxeEpcsEpaLogRx()
        {
            //arrange
            var license = new ApplicationLicense { DeluxePricingStructure = Constants.DeluxePricingStructure.DeluxeEpcsEpaLogRx };

            //act
            var result = TieUtility.IsLicenseAdsEnabled(license);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_true_if_license_is_LegacyDeluxeEpaLogRx()
        {
            //arrange
            var license = new ApplicationLicense { DeluxePricingStructure = Constants.DeluxePricingStructure.LegacyDeluxeEpaLogRx };

            //act
            var result = TieUtility.IsLicenseAdsEnabled(license);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_true_if_license_is_LegacyDeluxeEpcsLogRx()
        {
            //arrange
            var license = new ApplicationLicense { DeluxePricingStructure = Constants.DeluxePricingStructure.LegacyDeluxeEpcsLogRx };

            //act
            var result = TieUtility.IsLicenseAdsEnabled(license);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_false_if_license_is_DeluxeEpcs()
        {
            //arrange
            var license = new ApplicationLicense { DeluxePricingStructure = Constants.DeluxePricingStructure.DeluxeEpcs };

            //act
            var result = TieUtility.IsLicenseAdsEnabled(license);

            //assert
            Assert.IsFalse(result);
        }
    }
}
