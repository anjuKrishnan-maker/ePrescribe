using System;
using System.Collections.Generic;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Objects;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using Allscripts.Impact.Tasks;
using Allscripts.Impact.Tasks.Interfaces;
using eRxWeb.AppCode;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using eRxWeb.AppCode.Tasks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.ApproveRefillTaskTests
{
    [TestClass]
    public class GetDispensedLabelTextTests
    {
        [TestMethod]
        public void should_call_GetFormattedRequestedRxDetailsTableText_if_type_is_rxchange()
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
                NDCNumber = "ndc12342",
                Notes = "NotesNotes",
                Quantity = 3,
                Refills = null,
                SigText = "sigtext....."
            };

            var requestList = new List<RequestedRx> {requestedRx2, requestedRx};

            var info = new ChangeRxSmInfo(null, Guid.Empty, new ChgRxPrescriberAuth(new ChangeRequestSubCodes())) { SubType = new RxChangeSubType("G") };

            var changeRxMock = MockRepository.GenerateMock<IChangeRx>();

            var sessionMock = MockRepository.GenerateStub<IStateContainer>();

            changeRxMock.Expect(x => x.GetFormattedRequestedRxDetailsTableText(null, null, false, false, null, null, null, null, null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return("");
            
            //act
            var result = ChangeRxTask.GetDispensedLabelText(Constants.PrescriptionTaskType.RXCHG, info, null, new PharmacyTaskRowControlValues(), true, true, "NY", "NY", changeRxMock, new ChgRxPrescriberAuth(new ChangeRequestSubCodes()), null, ConnectionStringPointer.AUDIT_ERXDB_SERVER_1);

            //assert
            changeRxMock.VerifyAllExpectations();
        }

        [TestMethod]
        public void should_call_GetFormattedDispensedRxDetailsText_if_type_is_not_rxchange()
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
                NDCNumber = "ndc12342",
                Notes = "NotesNotes",
                Quantity = 3,
                Refills = null,
                SigText = "sigtext....."
            };

            var requestList = new List<RequestedRx> { requestedRx2, requestedRx };

            var info = new ChangeRxSmInfo(null, Guid.Empty, new ChgRxPrescriberAuth(new ChangeRequestSubCodes())){ SubType = new RxChangeSubType("G")};

            var changeRxMock = MockRepository.GenerateMock<IChangeRx>();

            changeRxMock.Expect(x => x.GetFormattedDispensedRxDetailsText(null, null, null, null, null, null, null, null, Constants.PrescriptionTaskType.APPROVAL_REQUEST)).IgnoreArguments().Return("");

            var sessionMock = MockRepository.GenerateStub<IStateContainer>();

            //act
            var result = ChangeRxTask.GetDispensedLabelText((int)Constants.PrescriptionTaskType.APPROVAL_REQUEST, info, new DispensedRx(), new PharmacyTaskRowControlValues(), true, true, "NY", "NY", changeRxMock, new ChgRxPrescriberAuth(new ChangeRequestSubCodes()), null, ConnectionStringPointer.AUDIT_ERXDB_SERVER_1);

            //assert
            changeRxMock.VerifyAllExpectations();
        }
    }
}
