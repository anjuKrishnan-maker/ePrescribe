using System;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Allscripts.ePrescribe.Common;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.TieUtilityTests
{
    [TestClass]
    public class ShouldShowAdsTests
    {
        [TestMethod]
        public void should_return_true_if_showAdPlacement_is_true_isLicenseLogRxEnabled_is_false_and_deluxeFeatureDisplay_does_not_contain_platinum()
        {
            //arrange
            var showAdPlacement = true;
            var isLicesnseAdsEnabled = false;
            var deluxeFeatureDisplay = "Basic";
            var enterpriseId = Guid.NewGuid();

            //act
            var result = TieUtility.ShouldShowAds(showAdPlacement, isLicesnseAdsEnabled, deluxeFeatureDisplay, enterpriseId, Constants.DeluxeFeatureStatus.Off);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_true_if_showAdPlacement_is_false_isLicenseLogRxEnabled_is_true_and_deluxeFeatureDisplay_does_not_contain_platinum()
        {
            //arrange
            var showAdPlacement = false;
            var isLicesnseAdsEnabled = true;
            var deluxeFeatureDisplay = "Basic";
            var enterpriseId = Guid.NewGuid();

            //act
            var result = TieUtility.ShouldShowAds(showAdPlacement, isLicesnseAdsEnabled, deluxeFeatureDisplay, enterpriseId, Constants.DeluxeFeatureStatus.Off);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_true_if_showAd_is_true_isLicenseLogRxEnabled_is_true_and_deluxeFeatureDisplay_does_not_contain_platinum()
        {
            //arrange
            var showAdPlacement = true;
            var isLicesnseAdsEnabled = true;
            var deluxeFeatureDisplay = "Platinum";
            var enterpriseId = Guid.NewGuid();

            //act
            var result = TieUtility.ShouldShowAds(showAdPlacement, isLicesnseAdsEnabled, deluxeFeatureDisplay, enterpriseId, Constants.DeluxeFeatureStatus.Off);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_true_if_showAdPlacement_is_true_isLicenseLogRxEnabled_is_false_and_deluxeFeatureDisplay_does_contain_platinum_and_enterpriseId_equals_deluxeSalesId()
        {
            //arrange
            var showAdPlacement = true;
            var isLicesnseAdsEnabled = false;
            var deluxeFeatureDisplay = "Platinum";
            var enterpriseId = new Guid("F702FABB-C77E-4F49-9F1E-50C7B844BBA6");

            //act
            var result = TieUtility.ShouldShowAds(showAdPlacement, isLicesnseAdsEnabled, deluxeFeatureDisplay, enterpriseId, Constants.DeluxeFeatureStatus.Off);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_true_if_showAdPlacement_is_false_isLicenseLogRxEnabled_is_true_and_deluxeFeatureDisplay_does_contain_platinum_and_enterpriseId_equals_deluxeSalesId()
        {
            //arrange
            var showAdPlacement = false;
            var isLicesnseAdsEnabled = true;
            var deluxeFeatureDisplay = "Platinum";
            var enterpriseId = new Guid("F702FABB-C77E-4F49-9F1E-50C7B844BBA6");

            //act
            var result = TieUtility.ShouldShowAds(showAdPlacement, isLicesnseAdsEnabled, deluxeFeatureDisplay, enterpriseId, Constants.DeluxeFeatureStatus.Off);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_true_if_showAd_is_true_isLicenseLogRxEnabled_is_true_and_deluxeFeatureDisplay_does_contain_platinum_and_enterpriseId_equals_deluxeSalesId()
        {
            //arrange
            var showAdPlacement = true;
            var isLicesnseAdsEnabled = true;
            var deluxeFeatureDisplay = "Platinum";
            var enterpriseId = new Guid("F702FABB-C77E-4F49-9F1E-50C7B844BBA6");

            //act
            var result = TieUtility.ShouldShowAds(showAdPlacement, isLicesnseAdsEnabled, deluxeFeatureDisplay, enterpriseId, Constants.DeluxeFeatureStatus.Off);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_true_if_showAd_is_true_isLicenseLogRxEnabled_is_false_and_deluxeFeatureDisplay_contains_platinum_and_enterpriseId_does_not_equal_deluxeSalesId()
        {
            //arrange
            var showAdPlacement = true;
            var isLicesnseAdsEnabled = false;
            var deluxeFeatureDisplay = "Platinum";
            var enterpriseId = Guid.NewGuid();

            //act
            var result = TieUtility.ShouldShowAds(showAdPlacement, isLicesnseAdsEnabled, deluxeFeatureDisplay, enterpriseId, Constants.DeluxeFeatureStatus.Off);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_false_if_showAd_is_false_isLicenseLogRxEnabled_is_false_and_deluxeFeatureDisplay_does_not_contain_platinum_and_enterpriseId_equals_deluxeSalesId()
        {
            //arrange
            var showAdPlacement = false;
            var isLicesnseAdsEnabled = false;
            var deluxeFeatureDisplay = "Platinum";
            var enterpriseId = new Guid("F702FABB-C77E-4F49-9F1E-50C7B844BBA6");

            //act
            var result = TieUtility.ShouldShowAds(showAdPlacement, isLicesnseAdsEnabled, deluxeFeatureDisplay, enterpriseId, Constants.DeluxeFeatureStatus.Off);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_false_if_showAd_is_false_isLicenseLogRxEnabled_is_false_and_deluxeFeatureDisplay_does_contain_platinum_and_enterpriseId_does_not_equal_deluxeSalesId()
        {
            //arrange
            var showAdPlacement = false;
            var isLicesnseAdsEnabled = false;
            var deluxeFeatureDisplay = "Platinum";
            var enterpriseId = Guid.NewGuid();

            //act
            var result = TieUtility.ShouldShowAds(showAdPlacement, isLicesnseAdsEnabled, deluxeFeatureDisplay, enterpriseId, Constants.DeluxeFeatureStatus.Off);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_false_if_showAd_is_false_isLicenseLogRxEnabled_is_false_and_deluxeFeatureDisplay_does_contain_platinum_and_enterpriseId_does_equal_deluxeSalesId()
        {
            //arrange
            var showAdPlacement = false;
            var isLicesnseAdsEnabled = false;
            var deluxeFeatureDisplay = "Platinum";
            var enterpriseId = new Guid("F702FABB-C77E-4F49-9F1E-50C7B844BBA6");

            //act
            var result = TieUtility.ShouldShowAds(showAdPlacement, isLicesnseAdsEnabled, deluxeFeatureDisplay, enterpriseId, Constants.DeluxeFeatureStatus.Off);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_true_if_showAdPlacement_is_false_isLicenseLogRxEnabled_is_true_deluxeFeatureDisplay_does_not_contain_platinum_enterpiseIsCompulsoryBasic()
        {
            //arrange
            var showAdPlacement = false;
            var isLicesnseAdsEnabled = true;
            var deluxeFeatureDisplay = "Basic";
            var enterpriseId = Guid.NewGuid();

            //act
            var result = TieUtility.ShouldShowAds(showAdPlacement, isLicesnseAdsEnabled, deluxeFeatureDisplay, enterpriseId, Constants.DeluxeFeatureStatus.CompulsoryBasic);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_true_if_showAdPlacement_is_false_isLicenseLogRxEnabled_is_true_deluxeFeatureDisplay_does_not_contain_platinum_enterpiseIsForceCompulsoryBasic()
        {
            //arrange
            var showAdPlacement = false;
            var isLicesnseAdsEnabled = true;
            var deluxeFeatureDisplay = "Basic";
            var enterpriseId = Guid.NewGuid();

            //act
            var result = TieUtility.ShouldShowAds(showAdPlacement, isLicesnseAdsEnabled, deluxeFeatureDisplay, enterpriseId, Constants.DeluxeFeatureStatus.ForceCompulsoryBasic);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_true_if_showAdPlacement_is_false_isLicenseLogRxEnabled_is_true_and_deluxeFeatureDisplay_does_contain_platinum_and_enterpriseId_equals_compulsoryBasicSales()
        {
            //arrange
            var showAdPlacement = false;
            var isLicesnseAdsEnabled = true;
            var deluxeFeatureDisplay = "Platinum";
            var enterpriseId = new Guid("6E55DB44-D9A8-4925-A654-A63708CF34F9");

            //act
            var result = TieUtility.ShouldShowAds(showAdPlacement, isLicesnseAdsEnabled, deluxeFeatureDisplay, enterpriseId, Constants.DeluxeFeatureStatus.Off);

            //assert
            Assert.IsTrue(result);
        }
    }
}
