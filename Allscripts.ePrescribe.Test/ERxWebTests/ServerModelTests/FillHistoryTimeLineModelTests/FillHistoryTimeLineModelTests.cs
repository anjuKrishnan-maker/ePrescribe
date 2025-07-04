using System;
using Allscripts.ePrescribe.Objects.StoredProcReturnObjects;
using eRxWeb.ServerModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.ServerModelTests.FillHistoryTimeLineModelTests
{
    [TestClass]
    public class FillHistoryTimeLineModelTests
    {
        [TestMethod]
        public void should_set_pharmname_to_unknown_if_pharm_name_is_null()
        {
            //arrange
            var fillRecord = new GetFillDatesForRxIdRow();

            //act
            var result = new FillHistoryTimeLineModel(fillRecord);

            //assert
            Assert.AreEqual("RxFill ,  Pharmacy: Unknown", result.TimeLineLabel);
        }

        [TestMethod]
        public void should_set_pharmname_to_unknown_if_pharm_name_is_whitespace()
        {
            //arrange
            var fillRecord = new GetFillDatesForRxIdRow
            { 
                PharmName = "  "
            };

            //act
            var result = new FillHistoryTimeLineModel(fillRecord);

            //assert
            Assert.AreEqual("RxFill ,  Pharmacy: Unknown", result.TimeLineLabel);
        }

        [TestMethod]
        public void should_set_timeLineLabel_for_rxFill_if_pbmSource_is_null()
        {
            //arrange
            var pharmName = "CVS Pharmacies";

            var fillRecord = new GetFillDatesForRxIdRow
            {
                PharmName = pharmName
            };

            //act
            var result = new FillHistoryTimeLineModel(fillRecord);

            //assert
            Assert.AreEqual($"RxFill ,  Pharmacy: {pharmName}", result.TimeLineLabel);
        }

        [TestMethod]
        public void should_set_timeLineLabel_for_rxFill_if_pbmSource_is_whitespace()
        {
            //arrange
            var pharmName = "CVS Pharmacies";
            var rxFillStatus = "Dispensed Partial";
            var message = "12 pills filled";

            var fillRecord = new GetFillDatesForRxIdRow
            {
                PharmName = pharmName,
                PBMSource = "  ",
                Message = message,
                RxFillStatus = rxFillStatus
            };

            //act
            var result = new FillHistoryTimeLineModel(fillRecord);

            //assert
            Assert.AreEqual($"RxFill {rxFillStatus}, {message} Pharmacy: {pharmName}", result.TimeLineLabel);
        }

        [TestMethod]
        public void should_set_label_for_pbm_history_if_pbmSource_is_populated()
        {
            //arrange
            var pharmName = "CVS Pharmacies";
            var pbmSource = "Surescripts";

            var fillRecord = new GetFillDatesForRxIdRow
            {
                PharmName = pharmName,
                PBMSource = pbmSource
            };

            //act
            var result = new FillHistoryTimeLineModel(fillRecord);

            //assert
            Assert.AreEqual($"{pbmSource}, Pharmacy: {pharmName}", result.TimeLineLabel);
        }

        [TestMethod]
        public void should_use_shortdate_for_fillDate()
        {
            //arrange
            var fillDate = DateTime.Today;

            var fillRecord = new GetFillDatesForRxIdRow
            {
                FillDate = fillDate
            };

            //act
            var result = new FillHistoryTimeLineModel(fillRecord);

            //assert
            Assert.AreEqual(fillDate.ToShortDateString(), result.FillDate);
        }
    }
}
