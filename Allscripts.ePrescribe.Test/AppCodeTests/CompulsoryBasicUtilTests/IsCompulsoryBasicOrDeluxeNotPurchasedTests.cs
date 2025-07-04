using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.AppCodeTests.CompulsoryBasicUtilTests
{
    public partial class CompulsoryBasicUtilTests
    {
        [TestClass]
        public class IsCompulsoryBasicOrDeluxeNotPurchasedTests
        {
            [TestMethod]
            public void should_return_true_for_enterprise_compulosorybasic_pricingstructure_basic()
            {
                //arrange
                bool isEnterpriseCompulsoryBasic = true;
                bool isPricingStructureBasic = true;

                //act
                bool result = eRxWeb.AppCode.CompulsoryBasicUtil.IsCompulsoryBasicOrDeluxeNotPurchased(isEnterpriseCompulsoryBasic, isPricingStructureBasic);

                //assert
                bool expected = true;
                Assert.AreEqual(expected, result);
            }

            [TestMethod]
            public void should_return_false_for_enterprise_notcompulosorybasic_pricingstructure_basic()
            {
                //arrange
                bool isEnterpriseCompulsoryBasic = false;
                bool isPricingStructureBasic = true;

                //act
                bool result = eRxWeb.AppCode.CompulsoryBasicUtil.IsCompulsoryBasicOrDeluxeNotPurchased(isEnterpriseCompulsoryBasic, isPricingStructureBasic);

                //assert
                bool expected = false;
                Assert.AreEqual(expected, result);
            }

            [TestMethod]
            public void should_return_false_for_enterprise_compulosorybasic_pricingstructure_notbasic()
            {
                //arrange
                bool isEnterpriseCompulsoryBasic = true;
                bool isPricingStructureBasic = false;

                //act
                bool result = eRxWeb.AppCode.CompulsoryBasicUtil.IsCompulsoryBasicOrDeluxeNotPurchased(isEnterpriseCompulsoryBasic, isPricingStructureBasic);

                //assert
                bool expected = false;
                Assert.AreEqual(expected, result);
            }

        }
    }
}