using System;
using Allscripts.Impact.ePrescribeSvc;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.CancelRxTests
{
    [TestClass]
    public class IsScriptCancelRxEligileTests
    {
        private Guid trnCtrlNo => new Guid("A2F1C600-3095-4863-974F-B30B23E23B74");

        [TestMethod]
        public void should_return_false_if_transMethod_is_not_S_and_isPharmacyCancelRxEligible_is_Y()
        {
            //arrange
            var transMethod = "T";
            var isPharmacyCancelRxEligible = "Y";

            //act
            var result = CancelRx.IsScriptCancelRxEligile(transMethod, "", "", isPharmacyCancelRxEligible, trnCtrlNo);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_false_if_transMethod_is_S_and_isPharmacyCancelRxEligible_is_N()
        {
            //arrange
            var transMethod = "S";
            var isPharmacyCancelRxEligible = "N";

            //act
            var result = CancelRx.IsScriptCancelRxEligile(transMethod, "", "", isPharmacyCancelRxEligible, trnCtrlNo);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_false_if_transMethod_is_not_S_and_isPharmacyCancelRxEligible_is_N()
        {
            //arrange
            var transMethod = "J";
            var isPharmacyCancelRxEligible = "N";

            //act
            var result = CancelRx.IsScriptCancelRxEligile(transMethod, "", "", isPharmacyCancelRxEligible, trnCtrlNo);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_false_if_startDate_is_null()
        {
            //arrange
            var transMethod = "S";
            var isPharmacyCancelRxEligible = "Y";

            //act
            var result = CancelRx.IsScriptCancelRxEligile(transMethod, null, "", isPharmacyCancelRxEligible, trnCtrlNo);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_false_if_startDate_is_empty()
        {
            //arrange
            var transMethod = "S";
            var isPharmacyCancelRxEligible = "Y";

            //act
            var result = CancelRx.IsScriptCancelRxEligile(transMethod, "", "", isPharmacyCancelRxEligible, trnCtrlNo);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_false_if_startDate_is_not_a_valid_datetime()
        {
            //arrange
            var transMethod = "S";
            var isPharmacyCancelRxEligible = "Y";

            //act
            var result = CancelRx.IsScriptCancelRxEligile(transMethod, "NotADate", "", isPharmacyCancelRxEligible, trnCtrlNo);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_true_if_controlledSubstanceCode_is_empty_and_startDate_is_less_than_1_year()
        {
            //arrange
            var transMethod = "S";
            var isPharmacyCancelRxEligible = "Y";
            var startDate = DateTime.Today.AddYears(-1).AddHours(+1);

            //act
            var result = CancelRx.IsScriptCancelRxEligile(transMethod, startDate.ToString(), "", isPharmacyCancelRxEligible, trnCtrlNo);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_false_if_controlledSubstanceCode_is_empty_and_startDate_is_more_than_1_year()
        {
            //arrange
            var transMethod = "S";
            var isPharmacyCancelRxEligible = "Y";
            var startDate = DateTime.Today.AddYears(-1).AddHours(-1);

            //act
            var result = CancelRx.IsScriptCancelRxEligile(transMethod, startDate.ToString(), "", isPharmacyCancelRxEligible, trnCtrlNo);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_false_if_controlledSubstanceCode_is_not_empty_and_startDate_is_more_than_6_months()
        {
            //arrange
            var transMethod = "S";
            var isPharmacyCancelRxEligible = "Y";
            var startDate = DateTime.Today.AddMonths(-6).AddHours(-1);
            var controlledSubstanceCode = "2";

            //act
            var result = CancelRx.IsScriptCancelRxEligile(transMethod, startDate.ToString(), controlledSubstanceCode, isPharmacyCancelRxEligible, trnCtrlNo);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_true_if_controlledSubstanceCode_is_not_empty_and_startDate_is_less_than_6_months()
        {
            //arrange
            var transMethod = "S";
            var isPharmacyCancelRxEligible = "Y";
            var startDate = DateTime.Today.AddMonths(-6).AddHours(+1);
            var controlledSubstanceCode = "2";

            //act
            var result = CancelRx.IsScriptCancelRxEligile(transMethod, startDate.ToString(), controlledSubstanceCode, isPharmacyCancelRxEligible, trnCtrlNo);

            //assert
            Assert.IsTrue(result);
        }
    }
}