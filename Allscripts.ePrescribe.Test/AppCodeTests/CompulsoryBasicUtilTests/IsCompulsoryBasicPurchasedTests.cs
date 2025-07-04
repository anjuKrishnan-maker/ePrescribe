using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.AppCodeTests.CompulsoryBasicUtilTests
{
    public partial class CompulsoryBasicUtilTests
    {
        [TestClass]
        public class IsCompulsoryBasicPurchasedTests
        {
            [TestMethod]
            public void should_return_true_for_enterprise_compulosorybasic_pricingstructure_compulosorybasic()
            {
                //arrange
                bool isEnterpriseCompulsoryBasic = true;
                bool isPricingStructureCompulsoryBasic = true;

                //act
                bool result = eRxWeb.AppCode.CompulsoryBasicUtil.IsCompulsoryBasicPurchased(isEnterpriseCompulsoryBasic, isPricingStructureCompulsoryBasic);

                //assert
                bool expected = true;
                Assert.AreEqual(expected, result);
            }

            [TestMethod]
            public void should_return_false_for_enterprise_notcompulosorybasic_pricingstructure_compulosorybasic()
            {
                //arrange
                bool isEnterpriseCompulsoryBasic = false;
                bool isPricingStructureCompulsoryBasic = true;

                //act
                bool result = eRxWeb.AppCode.CompulsoryBasicUtil.IsCompulsoryBasicPurchased(isEnterpriseCompulsoryBasic, isPricingStructureCompulsoryBasic);

                //assert
                bool expected = false;
                Assert.AreEqual(expected, result);
            }

            [TestMethod]
            public void should_return_false_for_enterprise_compulosorybasic_pricingstructure_notcompulosorybasic()
            {
                //arrange
                bool isEnterpriseCompulsoryBasic = true;
                bool isPricingStructureCompulsoryBasic = false;

                //act
                bool result = eRxWeb.AppCode.CompulsoryBasicUtil.IsCompulsoryBasicPurchased(isEnterpriseCompulsoryBasic, isPricingStructureCompulsoryBasic);

                //assert
                bool expected = false;
                Assert.AreEqual(expected, result);
            }
        }
    }
}
