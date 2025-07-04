using System;
using Allscripts.ePrescribe.Objects;
using Allscripts.Impact;
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
    public class BuildDetailsBlockTcTests
    {
        [TestMethod]
        public void should_create_tabel_cell_with_details_as_literal()
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
            var prescriptionModel = new TaskRxDetailsModel();
            prescriptionModel.PharmacyDetails = "CVS Raleigh, NC";
            prescriptionModel.DrugDescription = "Sodium Oxybate";
            var mockSession = MockRepository.GenerateStub<IStateContainer>();

            //act
            var result = ChangeRxTask.BuildDetailsBlockTc(subType, 12, requestedRx, Guid.NewGuid(), Medication.MedType.CS, true, true, prescriptionModel);
             
            //assert
            Assert.AreEqual(UiHelper.RenderControl(result).StartsWith("<td style=\"border: none !important;\"><span style=\"color:Red;\">EPCS not enabled.</span><br><b>Requested Medication:</b><b><BR>Rx Detail: </b>new drug&nbsp;&nbsp;<img src=\"~/images/ControlSubstance_sm.gif\" alt=\"CS Indicator\" /><BR><b>SIG: </b>sigtext.....<BR><b>Total number of dispenses requested:</b> 12, <b>Quantity:</b> 3<BR><b>Notes: NotesNotes&nbsp;&nbsp;&nbsp;</b><BR><b>Original Rx Date:</b> 1/22/2016&nbsp;&nbsp;&nbsp;<b>Last Fill Date:</b> 1/23/2016&nbsp;&nbsp;&nbsp;<BR><a onclick=\"SetGridBindRequirment();\" type=\"RXCHG\" href=\"RedirectToAngular.aspx?componentName=SelectMedication&amp;componentParameters={%22ChangeRxPharmacy%22%3a+%22CVS+Raleigh%2c+NC%22%2c%22ScriptMessageGuid%22%3a+%22"), true);
        }
    }
}
