using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Allscripts.ePrescribe.Test.AppCodeTests.CompulsoryBasicUtilTests
{
    [TestClass]
    public partial class CompulsoryBasicUtilTests
    {
        [TestClass]
        public class DisplayPromoTeaserAdTests
        {
            [TestMethod]
            public void should_return_true_when_enterprise_compulsorybasic_pricingstructure_basic_startdate_future()
            {
                //arrange
                bool isEnterpriseCompulsoryBasic = true;
                bool isPricingStructureBasic = true;
                DateTime startDate = DateTime.Now.AddDays(1);

                //act
                bool result = eRxWeb.AppCode.CompulsoryBasicUtil.DisplayPromoTeaserAd(isEnterpriseCompulsoryBasic, isPricingStructureBasic, startDate);

                //assert
                bool expected = true;
                Assert.AreEqual(expected, result);
            }

            [TestMethod]
            public void should_return_false_when_enterprise_compulsorybasic_pricingstructure_basic_startdate_past()
            {
                //arrange
                bool isEnterpriseCompulsoryBasic = true;
                bool isPricingStructureBasic = true;
                DateTime startDate = DateTime.Now.AddDays(-1);

                //act
                bool result = eRxWeb.AppCode.CompulsoryBasicUtil.DisplayPromoTeaserAd(isEnterpriseCompulsoryBasic, isPricingStructureBasic, startDate);

                //assert
                bool expected = false;
                Assert.AreEqual(expected, result);
            }

            [TestMethod]
            public void should_return_false_when_enterprise_notcompulsorybasic_pricingstructure_basic_startdate_future()
            {
                //arrange
                bool isEnterpriseCompulsoryBasic = false;
                bool isPricingStructureBasic = true;
                DateTime startDate = DateTime.Now.AddDays(1);

                //act
                bool result = eRxWeb.AppCode.CompulsoryBasicUtil.DisplayPromoTeaserAd(isEnterpriseCompulsoryBasic, isPricingStructureBasic, startDate);

                //assert
                bool expected = false;
                Assert.AreEqual(expected, result);
            }

            [TestMethod]
            public void should_return_false_when_enterprise_compulsorybasic_pricingstructure_notbasic_startdate_future()
            {
                //arrange
                bool isEnterpriseCompulsoryBasic = true;
                bool isPricingStructureBasic = false;
                DateTime startDate = DateTime.Now.AddDays(1);

                //act
                bool result = eRxWeb.AppCode.CompulsoryBasicUtil.DisplayPromoTeaserAd(isEnterpriseCompulsoryBasic, isPricingStructureBasic, startDate);

                //assert
                bool expected = false;
                Assert.AreEqual(expected, result);
            }
        }
    }
}
