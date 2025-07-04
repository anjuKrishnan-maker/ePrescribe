using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Allscripts.ePrescribe.Test.AppCodeTests.CompulsoryBasicUtilTests
{
    public partial class CompulsoryBasicUtilTests
    {
        [TestClass]
        public class IsLicenseInFreeTrialPeriod
        {
            [TestMethod]
            public void should_return_true_for_createdDate_lessThan_trialDays()
            {
                //arrange
                DateTime licenseCreationDate = DateTime.Now.AddDays(-25);
                int trialPeriodDays = 30;

                //act
                bool result = eRxWeb.AppCode.CompulsoryBasicUtil.IsLicenseInFreeTrialPeriod(licenseCreationDate, trialPeriodDays);

                //assert
                bool expected = true;
                Assert.AreEqual(expected, result);
            }

            [TestMethod]
            public void should_return_false_for_createdDate_moreThan_trialDays()
            {
                //arrange
                DateTime licenseCreationDate = DateTime.Now.AddDays(-35);
                int trialPeriodDays = 30;

                //act
                bool result = eRxWeb.AppCode.CompulsoryBasicUtil.IsLicenseInFreeTrialPeriod(licenseCreationDate, trialPeriodDays);

                //assert
                bool expected = false;
                Assert.AreEqual(expected, result);
            }

            [TestMethod]
            public void should_return_false_when_licenseCreatedToday_trialDaysZero()
            {
                //arrange
                DateTime licenseCreationDate = DateTime.Now;
                int trialPeriodDays = 0;

                //act
                bool result = eRxWeb.AppCode.CompulsoryBasicUtil.IsLicenseInFreeTrialPeriod(licenseCreationDate, trialPeriodDays);

                //assert
                bool expected = false;
                Assert.AreEqual(expected, result);
            }
        }
    }
}
