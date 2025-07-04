using System;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.TaskDisplayTests
{
    [TestClass]
    public class GetRefillTextTests
    {
        [TestMethod]
        public void should_return_Not_specified_if_both_qty_and_qtyQual_is_null()
        {
            //act
            var result = TaskDisplay.GetRefillText(null, null);

            //assert
            Assert.AreEqual(CommonTerms.NotSpecified, result);
        }

        [TestMethod]
        public void should_return_Not_specified_if_qty_is_not_a_number_and_qtyQual_is_null()
        {
            //act
            var result = TaskDisplay.GetRefillText("Refills", null);

            //assert
            Assert.AreEqual(CommonTerms.NotSpecified, result);
        }

        [TestMethod]
        public void should_return_not_specified_if_qtyQual_is_null_and_qty_is__not_a_number()
        {
            //act
            var result = TaskDisplay.GetRefillText("MeNumero", null);

            //assert
            Assert.AreEqual(CommonTerms.NotSpecified, result);
        }

        [TestMethod]
        public void should_return_qtyQual_if_qty_is_not_a_number()
        {
            //act
            var result = TaskDisplay.GetRefillText("MeNumero", "12");

            //assert
            Assert.AreEqual("12", result);
        }

        [TestMethod]
        public void should_return_refills_value_same_as_input_task()
        {
            //act
            var result = TaskDisplay.GetRefillText("3", null);

            //assert
            Assert.AreEqual("3", result);
        }

        [TestMethod]
        public void should_return_0_if_qty_is_greater_than_99()
        {
            //act
            var result = TaskDisplay.GetRefillText("100", null);

            //assert
            Assert.AreEqual("0", result);
        }
    }
}
