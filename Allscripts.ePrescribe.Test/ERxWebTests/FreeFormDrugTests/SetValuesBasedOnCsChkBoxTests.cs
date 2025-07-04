using System;
using Allscripts.Impact;
using eRxWeb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.FreeFormDrugTests
{
    [TestClass]
    public class SetValuesBasedOnCsChkBoxTests
    {
        [TestMethod]
        public void should_return_if_rx_is_null_and_not_throw_exception()
        {
            Rx rx = null;

            //assert
            FreeFormDrug.SetValuesBasedOnCsChkBox(ref rx, false, null);
        }

        [TestMethod]
        public void should_set_values_when_is_csMed_is_true()
        {
            //arrange
            var rx = new Rx();
            var isCsMed = true;
            var schedule = "2";

            //assert
            FreeFormDrug.SetValuesBasedOnCsChkBox(ref rx, isCsMed, schedule);

            //act
            Assert.AreEqual(isCsMed, rx.IsFreeFormMedControlSubstance);
            Assert.AreEqual(Convert.ToInt32(schedule), rx.ScheduleUsed);
            Assert.AreEqual(schedule, rx.ControlledSubstanceCode);
        }

        [TestMethod]
        public void should_set_values_when_is_csMed_is_false()
        {
            //arrange
            var rx = new Rx {IsFreeFormMedControlSubstance = true, ScheduleUsed = 2, ControlledSubstanceCode = "2"};
            var isCsMed = false;

            //assert
            FreeFormDrug.SetValuesBasedOnCsChkBox(ref rx, isCsMed, null);

            //act
            Assert.AreEqual(isCsMed, rx.IsFreeFormMedControlSubstance);
            Assert.AreEqual(default(int), rx.ScheduleUsed);
            Assert.AreEqual(default(string), rx.ControlledSubstanceCode);
        }
    }
}
