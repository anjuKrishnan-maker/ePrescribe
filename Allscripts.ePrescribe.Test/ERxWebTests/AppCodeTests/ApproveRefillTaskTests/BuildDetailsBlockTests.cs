using System;
using Allscripts.ePrescribe.Objects;
using Allscripts.Impact.Tasks;
using eRxWeb.AppCode;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using eRxWeb;
using Medication = Allscripts.Impact.Medication;
using eRxWeb.AppCode.Tasks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.ApproveRefillTaskTests
{
    [TestClass]
    public class BuildDetailsBlockTests
    {
        [TestMethod]
        public void should_add_change_request_button_if_therapeutic_subtype()
        {
            //arrange
            var requestedRx = new RequestedRx
            {
                CreateDate = "1/22/2016",
                Daw = "2",
                DaysSupply = 3,
                Description = "new drug",
                LastFillDate = "1/23/2016",
                NDCNumber = "ndc12342",
                Notes = "NotesNotes",
                Quantity = 3,
                Refills = "12",
                SigText = "sigtext....."
            };
            var subType = new RxChangeSubType("T");
            var sessionMock = MockRepository.GenerateStub<IStateContainer>();
            var prescriptionModel = new TaskRxDetailsModel();
            //act
            var guid = Guid.NewGuid();
            var result = ChangeRxTask.BuildDetailsBlock(subType, 1, requestedRx, guid, Medication.MedType.NON_CS, true, prescriptionModel);

            //assert
            Assert.AreEqual(
                "<br><b>Requested Medication:</b><b><BR>Rx Detail: </b>new drug<BR><b>SIG: </b>sigtext.....<BR><b>Total number of dispenses requested:</b> 12, <b>Quantity:</b> 3<BR><b>Notes: NotesNotes&nbsp;&nbsp;&nbsp;</b><BR><b>Original Rx Date:</b> 1/22/2016&nbsp;&nbsp;&nbsp;<b>Last Fill Date:</b> 1/23/2016&nbsp;&nbsp;&nbsp;<BR><a onclick=\"return SetGridBindRequirment();\" type=\"RXCHG\" href=\"Sig.aspx?reqRx=1&amp;smid="+guid+"&amp;tasktype=RXCHG\" style=\"color:Gray;font-weight:normal;\">Change Request</a>",
                result);
        }


        [TestMethod]
        public void should_not_add_change_request_button_if_not_therapeutic_subtype()
        {
            //arrange
            var requestedRx = new RequestedRx
            {
                CreateDate = "1/22/2016",
                Daw = "2",
                DaysSupply = 3,
                Description = "new drug",
                LastFillDate = "1/23/2016",
                NDCNumber = "ndc12342",
                Notes = "NotesNotes",
                Quantity = 3,
                Refills = "12",
                SigText = "sigtext....."
            };
            var subType = new RxChangeSubType("G");

            var sessionMock = MockRepository.GenerateStub<IStateContainer>();
            var prescriptionModel = new TaskRxDetailsModel();
            Guid guid = Guid.NewGuid();
            //act
            var result = ChangeRxTask.BuildDetailsBlock(subType, 1, requestedRx, guid, Medication.MedType.NON_CS, true, prescriptionModel);
            var shouldBe = "<br><b>Requested Medication:</b><b><BR>Rx Detail: </b>new drug<BR><b>SIG: </b>sigtext.....<BR><b>Total number of dispenses requested:</b> 12, <b>Quantity:</b> 3<BR><b>Notes: NotesNotes&nbsp;&nbsp;&nbsp;</b><BR><b>Original Rx Date:</b> 1/22/2016&nbsp;&nbsp;&nbsp;<b>Last Fill Date:</b> 1/23/2016&nbsp;&nbsp;&nbsp;<BR><a onclick=\"return SetGridBindRequirment();\" type=\"RXCHG\" href=\"Sig.aspx?reqRx=1&amp;smid="+guid+"&amp;tasktype=RXCHG\" style=\"color:Gray;font-weight:normal;\">Change Request</a>";
            //assert
            Assert.AreEqual(shouldBe, result);

        }
    }
}
