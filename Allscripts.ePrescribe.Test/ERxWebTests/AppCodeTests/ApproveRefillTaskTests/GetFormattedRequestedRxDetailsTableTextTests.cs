using System;
using System.Collections.Generic;
using System.Web.SessionState;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Objects;
using Allscripts.Impact;
using Allscripts.Impact.Tasks;
using Allscripts.Impact.Tasks.Interfaces;
using eRxWeb.AppCode;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Medication = Allscripts.Impact.Medication;
using eRxWeb;
using eRxWeb.AppCode.Tasks;
using IMedication = Allscripts.Impact.Interfaces.IMedication;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.ApproveRefillTaskTests
{
    [TestClass]
    public class GetFormattedRequestedRxDetailsTableTextTests
    {
        [TestMethod]
        public void should_create_table_with_individual_rows_of_requested_rxs()
        {
            //arrange
            var requestedRx = new RequestedRx
            {
                CreateDate = "1/22/2016",
                Daw = "2",
                DaysSupply = 3,
                Description = "new drug",
                LastFillDate = "1/23/2016",
                Ndc = "ndc12342",
                Notes = "NotesNotes",
                Quantity = 3,
                Refills = null,
                SigText = "sigtext....."
            };

            var requestedRx2 = new RequestedRx
            {
                CreateDate = "1/22/2016",
                Daw = "2",
                DaysSupply = 3,
                Description = "new drug",
                LastFillDate = "1/23/2016",
                Ndc = "ndc12342",
                Notes = "NotesNotes",
                Quantity = 3,
                Refills = null,
                SigText = "sigtext....."
            };
            var info = new ChangeRxSmInfo(null, Guid.Empty, new ChgRxPrescriberAuth(new ChangeRequestSubCodes()))
            {
                SubType = new RxChangeSubType("G"),
                RequestedRxs = new List<RequestedRx> { requestedRx, requestedRx2 }
            };

            var sessionMock = MockRepository.GenerateStub<IStateContainer>();

            var approveMock = MockRepository.GenerateMock<IApproveRefillTask>();
            approveMock.Stub(x => x.GetMedTypeEnum(null, null, null, null, null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(Medication.MedType.NON_CS);

            var medMock = MockRepository.GenerateMock<IMedication>();
            medMock.Stub(x => x.GetDDIFromNDC(null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return("kkds");

            //act
            var result = new ChangeRxTask().GetFormattedRequestedRxDetailsTableText(info, new PharmacyTaskRowControlValues { RdbNoClientId = "kdksd" }, true, true, "NY", "NY", approveMock, medMock, null, ConnectionStringPointer.ERXDB_SERVER_1);
            var shouldBe = "<table style=\"border:none !important;\">\r\n\t<tr style=\"border:none !important;\">\r\n\t\t<td class=\"valgn\" style=\"border: none !important;\"><span class=\"aspNetDisabled\"><input id=\"\" type=\"radio\" name=\"select\" value=\"requestedApprove0\" disabled=\"disabled\" onclick=\"enableProcessBtn(); ShowHideControls(kdksd, , , , );\" /><label for=\"\">Approve</label></span></td><td style=\"border: none !important;\"><br><b>Requested Medication:</b><b><BR>Rx Detail: </b>new drug<BR><b>SIG: </b>sigtext.....<BR><b>Total number of dispenses requested:</b> Not specified, <b>Quantity:</b> 3<BR><b>Notes: NotesNotes&nbsp;&nbsp;&nbsp;</b><BR><b>Original Rx Date:</b> 1/22/2016&nbsp;&nbsp;&nbsp;<b>Last Fill Date:</b> 1/23/2016&nbsp;&nbsp;&nbsp;<BR><a onclick=\"return SetGridBindRequirment();\" type=\"RXCHG\" href=\"Sig.aspx?reqRx=0&amp;smid=00000000-0000-0000-0000-000000000000&amp;tasktype=RXCHG\" style=\"color:Gray;font-weight:normal;\">Change Request</a></td>\r\n\t</tr><tr style=\"border:none !important;\">\r\n\t\t<td class=\"valgn\" style=\"border: none !important;\"><span class=\"aspNetDisabled\"><input id=\"\" type=\"radio\" name=\"select\" value=\"requestedApprove1\" disabled=\"disabled\" onclick=\"enableProcessBtn(); ShowHideControls(kdksd, , , , );\" /><label for=\"\">Approve</label></span></td><td style=\"border: none !important;\"><br><b>Requested Medication:</b><b><BR>Rx Detail: </b>new drug<BR><b>SIG: </b>sigtext.....<BR><b>Total number of dispenses requested:</b> Not specified, <b>Quantity:</b> 3<BR><b>Notes: NotesNotes&nbsp;&nbsp;&nbsp;</b><BR><b>Original Rx Date:</b> 1/22/2016&nbsp;&nbsp;&nbsp;<b>Last Fill Date:</b> 1/23/2016&nbsp;&nbsp;&nbsp;<BR><a onclick=\"return SetGridBindRequirment();\" type=\"RXCHG\" href=\"Sig.aspx?reqRx=1&amp;smid=00000000-0000-0000-0000-000000000000&amp;tasktype=RXCHG\" style=\"color:Gray;font-weight:normal;\">Change Request</a></td>\r\n\t</tr>\r\n</table>";

            //assert
            Assert.AreEqual(shouldBe, result);
        }

        [TestMethod]
        public void should_create_table_with_only_one_row_if_only_one_request()
        {
            //arrange
            var requestedRx = new RequestedRx
            {
                CreateDate = "1/22/2016",
                Daw = "2",
                DaysSupply = 3,
                Description = "new drug",
                LastFillDate = "1/23/2016",
                Ndc = "ndc12342",
                Notes = "NotesNotes",
                Quantity = 3,
                Refills = null,
                SigText = "sigtext....."
            };
            var info = new ChangeRxSmInfo(null, Guid.Empty, new ChgRxPrescriberAuth(new ChangeRequestSubCodes()))
            {
                SubType = new RxChangeSubType("G"),
                RequestedRxs = new List<RequestedRx> { requestedRx }
            };
            var sessionMock = MockRepository.GenerateStub<IStateContainer>();

            var approveMock = MockRepository.GenerateMock<IApproveRefillTask>();
            approveMock.Stub(x => x.GetMedTypeEnum(null, null, null, null, null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(Medication.MedType.NON_CS);

            var medMock = MockRepository.GenerateMock<IMedication>();
            medMock.Stub(x => x.GetDDIFromNDC(null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return("kkds");

            //act
            var result = new ChangeRxTask().GetFormattedRequestedRxDetailsTableText(info, new PharmacyTaskRowControlValues { RdbNoClientId = "kdkskd" }, true, true, "NY", "NY", approveMock, medMock, null, ConnectionStringPointer.AUDIT_ERXDB_SERVER_1);
            var shouldBe = "<table style=\"border:none !important;\">\r\n\t<tr style=\"border:none !important;\">\r\n\t\t<td class=\"valgn\" style=\"border: none !important;\"><span class=\"aspNetDisabled\"><input id=\"\" type=\"radio\" name=\"select\" value=\"requestedApprove0\" disabled=\"disabled\" onclick=\"enableProcessBtn(); ShowHideControls(kdkskd, , , , );\" /><label for=\"\">Approve</label></span></td><td style=\"border: none !important;\"><br><b>Requested Medication:</b><b><BR>Rx Detail: </b>new drug<BR><b>SIG: </b>sigtext.....<BR><b>Total number of dispenses requested:</b> Not specified, <b>Quantity:</b> 3<BR><b>Notes: NotesNotes&nbsp;&nbsp;&nbsp;</b><BR><b>Original Rx Date:</b> 1/22/2016&nbsp;&nbsp;&nbsp;<b>Last Fill Date:</b> 1/23/2016&nbsp;&nbsp;&nbsp;<BR><a onclick=\"return SetGridBindRequirment();\" type=\"RXCHG\" href=\"Sig.aspx?reqRx=0&amp;smid=00000000-0000-0000-0000-000000000000&amp;tasktype=RXCHG\" style=\"color:Gray;font-weight:normal;\">Change Request</a></td>\r\n\t</tr>\r\n</table>";
            //assert
            Assert.AreEqual(shouldBe, result);
        }

        [TestMethod]
        public void should_create_table_with_no_rows_if_invalid_ndc_sent()
        {
            //arrange
            var requestedRx = new RequestedRx
            {
                CreateDate = "1/22/2016",
                Daw = "2",
                DaysSupply = 3,
                Description = "new drug",
                LastFillDate = "1/23/2016",
                Ndc = "sadf",
                Notes = "NotesNotes",
                Quantity = 3,
                Refills = null,
                SigText = "sigtext....."
            };
            var info = new ChangeRxSmInfo(null, Guid.Empty, new ChgRxPrescriberAuth(new ChangeRequestSubCodes()))
            {
                SubType = new RxChangeSubType("G"),
                RequestedRxs = new List<RequestedRx> { requestedRx }
            };
            var sessionMock = MockRepository.GenerateStub<IStateContainer>();

            var approveMock = MockRepository.GenerateMock<IApproveRefillTask>();
            approveMock.Stub(x => x.GetMedTypeEnum(null, null, null, null, null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(Medication.MedType.NON_CS);

            var medMock = MockRepository.GenerateMock<IMedication>();
            medMock.Stub(x => x.GetDDIFromNDC(null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return("");

            //act
            var result = new ChangeRxTask().GetFormattedRequestedRxDetailsTableText(info, new PharmacyTaskRowControlValues { RdbNoClientId = "kdkskd" }, true, true, "NY", "NY", approveMock, medMock, null, ConnectionStringPointer.AUDIT_ERXDB_SERVER_1);
            var should = "<table style=\"border:none !important;\">\r\n\t<tr style=\"border:none !important;\">\r\n\t\t<td class=\"valgn\" style=\"border: none !important;\"><span class=\"aspNetDisabled\"><input id=\"\" type=\"radio\" name=\"select\" value=\"requestedApprove0\" disabled=\"disabled\" onclick=\"enableProcessBtn(); ShowHideControls(kdkskd, , , , );\" /><label for=\"\">Approve</label></span></td><td style=\"border: none !important;\"><br><b>Requested Medication:</b><b><BR>Rx Detail: </b>new drug<BR><b>SIG: </b>sigtext.....<BR><b>Total number of dispenses requested:</b> Not specified, <b>Quantity:</b> 3<BR><b>Notes: NotesNotes&nbsp;&nbsp;&nbsp;</b><BR><b>Original Rx Date:</b> 1/22/2016&nbsp;&nbsp;&nbsp;<b>Last Fill Date:</b> 1/23/2016&nbsp;&nbsp;&nbsp;<BR><a onclick=\"return SetGridBindRequirment();\" type=\"RXCHG\" href=\"Sig.aspx?reqRx=0&amp;smid=00000000-0000-0000-0000-000000000000&amp;tasktype=RXCHG\" style=\"color:Gray;font-weight:normal;\">Change Request</a></td>\r\n\t</tr>\r\n</table>";
            //assert
            Assert.AreEqual(should,
                result);
        }
        [TestMethod]
        public void should_allow_approve_since_refills_greater_than_zero()
        {
            //arrange
            var requestedRx = new RequestedRx
            {
                CreateDate = "1/22/2016",
                Daw = "2",
                DaysSupply = 3,
                Description = "new drug",
                LastFillDate = "1/23/2016",
                Ndc = "ndc12342",
                Notes = "NotesNotes",
                Quantity = 3,
                Refills = "5",
                SigText = "sigtext....."
            };
            var info = new ChangeRxSmInfo(null, Guid.Empty, new ChgRxPrescriberAuth(new ChangeRequestSubCodes()))
            {
                SubType = new RxChangeSubType("G"),
                RequestedRxs = new List<RequestedRx> { requestedRx }
            };
            var sessionMock = MockRepository.GenerateStub<IStateContainer>();

            var approveMock = MockRepository.GenerateMock<IApproveRefillTask>();
            approveMock.Stub(x => x.GetMedTypeEnum(null, null, null, null, null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(Medication.MedType.NON_CS);

            var medMock = MockRepository.GenerateMock<IMedication>();
            medMock.Stub(x => x.GetDDIFromNDC(null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return("kkds");

            //act
            var result = new ChangeRxTask().GetFormattedRequestedRxDetailsTableText(info, new PharmacyTaskRowControlValues { RdbNoClientId = "kdkskd" }, true, true, "NY", "NY", approveMock, medMock, null, ConnectionStringPointer.AUDIT_ERXDB_SERVER_1);
            var shouldBe = "<table style=\"border:none !important;\">\r\n\t<tr style=\"border:none !important;\">\r\n\t\t<td class=\"valgn\" style=\"border: none !important;\"><input id=\"\" type=\"radio\" name=\"select\" value=\"requestedApprove0\" onclick=\"enableProcessBtn(); ShowHideControls(kdkskd, , , , );\" /><label for=\"\">Approve</label></td><td style=\"border: none !important;\"><br><b>Requested Medication:</b><b><BR>Rx Detail: </b>new drug<BR><b>SIG: </b>sigtext.....<BR><b>Total number of dispenses requested:</b> 5, <b>Quantity:</b> 3<BR><b>Notes: NotesNotes&nbsp;&nbsp;&nbsp;</b><BR><b>Original Rx Date:</b> 1/22/2016&nbsp;&nbsp;&nbsp;<b>Last Fill Date:</b> 1/23/2016&nbsp;&nbsp;&nbsp;<BR><a onclick=\"return SetGridBindRequirment();\" type=\"RXCHG\" href=\"Sig.aspx?reqRx=0&amp;smid=00000000-0000-0000-0000-000000000000&amp;tasktype=RXCHG\" style=\"color:Gray;font-weight:normal;\">Change Request</a></td>\r\n\t</tr>\r\n</table>";
            //assert
            Assert.AreEqual(shouldBe, result);
        }
    }
}
