using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.UI.WebControls;

namespace Allscripts.ePrescribe.Test.AppCodeTests.DeluxeAccountUtilTests
{
    [TestClass]
    public class ConstructCancelReasonsTests
    {
        [TestMethod]
        public void should_contain_basic_in_item1_if_pricingstructure_compulsorybasic()
        {
            //Arrange
            Constants.DeluxePricingStructure pricingStructure = Constants.DeluxePricingStructure.CompulsoryBasic;
            RadioButtonList cancelReason = new RadioButtonList();

            //Act
            DeluxeAccountUtil.ConstructCancelReasons(pricingStructure, cancelReason);

            //Assert
            string expectedItem1 = "I liked the Basic features but it is too expensive";
            string actualItem1 = cancelReason.Items[0].Text;
            Assert.AreEqual(expectedItem1, actualItem1);
        }

        [TestMethod]
        public void should_contain_deluxe_in_item1_if_pricingstructure_notcompulsorybasic()
        {
            //Arrange
            Constants.DeluxePricingStructure pricingStructure = Constants.DeluxePricingStructure.Deluxe;
            RadioButtonList cancelReason = new RadioButtonList();

            //Act
            DeluxeAccountUtil.ConstructCancelReasons(pricingStructure, cancelReason);

            //Assert
            string expectedItem1 = "I liked the Deluxe features but it is too expensive";
            string actualItem1 = cancelReason.Items[0].Text;
            Assert.AreEqual(expectedItem1, actualItem1);
        }
    }
}
