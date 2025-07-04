using System;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode.Reports;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static eRxWeb.AppCode.Reports.ReportMenuUtil;

namespace Allscripts.ePrescribe.Test.AppCodeTests.ReportTests.ReportMenuUtilTests
{
    [TestClass]
    public class ShowDeluxeAdForReportTests
    {
        [TestMethod]
        public void Should_return_DisplayReport_when_deluxefeature_on()
        {
            //Arrange 
            Constants.DeluxeFeatureStatus gcdStatus = Constants.DeluxeFeatureStatus.On;
            ReportDisplayStatus expected = ReportDisplayStatus.DisplayReport;

            //Act
            var result = new ReportMenuUtil().ShowDeluxeAdForReport(gcdStatus);

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Should_return_DisplayDeluxeAd_when_deluxefeature_Disabled()
        {
            //Arrange 
            Constants.DeluxeFeatureStatus gcdStatus = Constants.DeluxeFeatureStatus.Disabled;
            ReportDisplayStatus expected = ReportDisplayStatus.DisplayDeluxeAd;

            //Act
            var result = new ReportMenuUtil().ShowDeluxeAdForReport(gcdStatus);

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Should_return_DisplayDeluxeAd_when_deluxefeature_Off()
        {
            //Arrange 
            Constants.DeluxeFeatureStatus gcdStatus = Constants.DeluxeFeatureStatus.Disabled;
            ReportDisplayStatus expected = ReportDisplayStatus.DisplayDeluxeAd;

            //Act
            var result = new ReportMenuUtil().ShowDeluxeAdForReport(gcdStatus);

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Should_return_HideReport_when_deluxefeature_Hide()
        {
            //Arrange 
            Constants.DeluxeFeatureStatus gcdStatus = Constants.DeluxeFeatureStatus.Hide;
            ReportDisplayStatus expected = ReportDisplayStatus.HideReport;

            //Act
            var result = new ReportMenuUtil().ShowDeluxeAdForReport(gcdStatus);

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Should_return_DisplayReport_when_deluxefeature_issomethingelse()
        {
            //Arrange 
            Constants.DeluxeFeatureStatus gcdStatus = Constants.DeluxeFeatureStatus.AlwaysOn;
            ReportDisplayStatus expected = ReportDisplayStatus.DisplayReport;

            //Act
            var result = new ReportMenuUtil().ShowDeluxeAdForReport(gcdStatus);

            //Assert
            Assert.AreEqual(expected, result);
        }
    }
}
